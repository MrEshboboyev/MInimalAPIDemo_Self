using MagicVillaCouponAPI_Self.Data;
using MagicVillaCouponAPI_Self.Models;
using MagicVillaCouponAPI_Self.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// get all coupons
app.MapGet("/api/coupon", (ILogger<Program> _logger)  => {
    _logger.Log(LogLevel.Information, "Getting All Coupons");
    return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons").Produces<IEnumerable<Coupon>>(200);


// get by id coupon
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(x => x.Id == id));
}).WithName("GetCouponById").Produces<Coupon>(200);


// create coupon
app.MapPost("/api/coupon", ([FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    if(String.IsNullOrEmpty(coupon_C_DTO.Name))
    {
        return Results.BadRequest("Invalid Id or Coupon Name");
    }

    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).First().Id + 1;
    CouponStore.couponList.Add(coupon);
    return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id}, coupon);
}).WithName("CreateCoupon").Produces<Coupon>(200).Produces(400);


app.UseHttpsRedirection();
app.Run();

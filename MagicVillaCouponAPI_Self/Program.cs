using MagicVillaCouponAPI_Self.Data;
using MagicVillaCouponAPI_Self.Models;
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
app.MapGet("/api/coupon", ()  => {
    return Results.Ok(CouponStore.couponList);
}).WithName("GetCoupons");


// get by id coupon
app.MapGet("/api/coupon/{id:int}", (int id) =>
{
    return Results.Ok(CouponStore.couponList.FirstOrDefault(x => x.Id == id));
}).WithName("GetCouponById");


// create coupon
app.MapPost("/api/coupon", ([FromBody] Coupon coupon) =>
{
    if(coupon.Id != 0)
    {
        return Results.BadRequest("ID will be zero");
    }

    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).First().Id + 1;
    CouponStore.couponList.Add(coupon);
    return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id}, coupon);
}).WithName("CreateCoupon");


app.UseHttpsRedirection();
app.Run();

using AutoMapper;
using FluentValidation;
using MagicVillaCouponAPI_Self;
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

// adding AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

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
app.MapGet("/api/coupon/{id:int}", (IMapper _mapper, int id) =>
{
    var coupon = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);    
    return Results.Ok(couponDTO);
}).WithName("GetCouponById").Produces<CouponDTO>(200);


// create coupon
app.MapPost("/api/coupon", (
    IMapper _mapper,
    IValidator<CouponCreateDTO> _validation,
    [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    var validationResult = _validation.ValidateAsync(coupon_C_DTO).GetAwaiter().GetResult();
    if(!validationResult.IsValid)
    {
        return Results.BadRequest(validationResult.Errors.FirstOrDefault().ToString());
    }

    // is coupon's Name unique
    if(CouponStore.couponList.FirstOrDefault(x => x.Name == coupon_C_DTO.Name) !=  null)
    {
        return Results.BadRequest("Coupon Name already exists");
    }

    // CouponCreateDTO to Coupon
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).First().Id + 1;
    CouponStore.couponList.Add(coupon);

    // Coupon to CouponDTO (for displaying coupon some fields)
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);
    return Results.CreatedAtRoute("GetCouponById", new { id = coupon.Id}, couponDTO);
}).WithName("CreateCoupon").Produces<CouponDTO>(200).Produces(400);


app.UseHttpsRedirection();
app.Run();

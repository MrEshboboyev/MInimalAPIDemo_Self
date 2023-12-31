using AutoMapper;
using FluentValidation;
using MagicVillaCouponAPI_Self;
using MagicVillaCouponAPI_Self.Data;
using MagicVillaCouponAPI_Self.Models;
using MagicVillaCouponAPI_Self.Models.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// adding AutoMapper
builder.Services.AddAutoMapper(typeof(MappingConfig));

// add validate services
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// get all coupons
app.MapGet("/api/coupon", (ILogger<Program> _logger)  => {
    // creating response object
    APIResponse response = new APIResponse();

    // logging information of method
    _logger.Log(LogLevel.Information, "Getting All Coupons");

    // initialize values for response
    response.IsSuccess = true;
    response.Result = CouponStore.couponList;
    response.StatusCode = HttpStatusCode.OK;

    // returning response
    return Results.Ok(response);
}).WithName("GetCoupons").Produces<APIResponse>(200);


// get by id coupon
app.MapGet("/api/coupon/{id:int}", (
    ILogger<Program> _logger, 
    IMapper _mapper, int id) =>
{
    // creating response object
    APIResponse response = new APIResponse();

    var coupon = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
    // Is coupon null
    if(coupon == null)
    {
        response.ErrorMessages.Add("Coupon can't be found");
        return Results.BadRequest(response);
    }

    // mapping Coupon to CouponDTO
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    // initialize values for response
    response.IsSuccess = true;
    response.Result = couponDTO;
    response.StatusCode = HttpStatusCode.OK;


    return Results.Ok(response);
}).WithName("GetCouponById").Produces<APIResponse>(200);


// create coupon
app.MapPost("/api/coupon", (
    IMapper _mapper,
    IValidator<CouponCreateDTO> _validation,
    [FromBody] CouponCreateDTO coupon_C_DTO) =>
{
    // creating response object
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = _validation.ValidateAsync(coupon_C_DTO).GetAwaiter().GetResult();
    if(!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    // is coupon's Name unique
    if(CouponStore.couponList.FirstOrDefault(x => x.Name == coupon_C_DTO.Name) !=  null)
    {
        response.ErrorMessages.Add("Coupon Name already exists");
        return Results.BadRequest(response);
    }

    // CouponCreateDTO to Coupon
    Coupon coupon = _mapper.Map<Coupon>(coupon_C_DTO);
    coupon.Id = CouponStore.couponList.OrderByDescending(x => x.Id).First().Id + 1;
    CouponStore.couponList.Add(coupon);

    // Coupon to CouponDTO (for displaying coupon some fields)
    CouponDTO couponDTO = _mapper.Map<CouponDTO>(coupon);

    // initialize values for response
    response.IsSuccess = true;
    response.Result = couponDTO;
    response.StatusCode = HttpStatusCode.Created;

    return Results.Ok(response);
}).WithName("CreateCoupon").Produces<APIResponse>(200).Produces(400);


// update coupon
app.MapPut("/api/coupon/{id:int}", (
    IValidator<CouponUpdateDTO> _validation,
    IMapper _mapper, int id, 
    [FromBody] CouponUpdateDTO coupon_U_DTO) =>
{
    // creating response object
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var validationResult = _validation.ValidateAsync(coupon_U_DTO).GetAwaiter().GetResult();
    if (!validationResult.IsValid)
    {
        response.ErrorMessages.Add(validationResult.Errors.FirstOrDefault().ToString());
        return Results.BadRequest(response);
    }

    // is coupon's Name unique
    if (CouponStore.couponList.FirstOrDefault(x => x.Name == coupon_U_DTO.Name) != null)
    {
        response.ErrorMessages.Add("Coupon Name already exists");
        return Results.BadRequest(response);
    }


    Coupon couponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == coupon_U_DTO.Id);
    // Is coupon null
    if (couponFromStore == null)
    {
        response.ErrorMessages.Add("Coupon can't be found");
        return Results.BadRequest(response);
    }
    couponFromStore.IsActive = coupon_U_DTO.IsActive;
    couponFromStore.Name = coupon_U_DTO.Name;
    couponFromStore.Percent = coupon_U_DTO.Percent;
    couponFromStore.LastUpdated = DateTime.Now;


    // initialize values for response
    response.IsSuccess = true;
    response.Result = _mapper.Map<CouponDTO>(couponFromStore);
    response.StatusCode = HttpStatusCode.OK;

    return Results.Ok(response);
}).WithName("UpdateCoupon").Produces<APIResponse>(200).Produces(400);

// delete coupon
app.MapDelete("/api/coupon/{id:int}", (int id) =>
{
    // creating response object
    APIResponse response = new() { IsSuccess = false, StatusCode = HttpStatusCode.BadRequest };

    var couponFromStore = CouponStore.couponList.FirstOrDefault(x => x.Id == id);
    if(couponFromStore == null)
    {
        response.ErrorMessages.Add("Invalid id");
        return Results.BadRequest(response);
    }

    CouponStore.couponList.Remove(couponFromStore);

    // initialize values for response
    response.IsSuccess = true;
    response.StatusCode = HttpStatusCode.NoContent;

    return Results.Ok(response);
}).WithName("DeleteCoupon").Produces<APIResponse>(200).Produces(400);

app.UseHttpsRedirection();
app.Run();

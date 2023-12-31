using AutoMapper;
using MagicVillaCouponAPI_Self.Models;
using MagicVillaCouponAPI_Self.Models.DTO;

namespace MagicVillaCouponAPI_Self
{
    public class MappingConfig : Profile
    {
        public MappingConfig() 
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
        }
    }
}

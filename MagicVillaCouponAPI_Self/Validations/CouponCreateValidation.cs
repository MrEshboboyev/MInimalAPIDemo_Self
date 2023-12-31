using FluentValidation;
using MagicVillaCouponAPI_Self.Models.DTO;
using System.Data;

namespace MagicVillaCouponAPI_Self.Validations
{
    public class CouponCreateValidation : AbstractValidator<CouponCreateDTO>
    {
        public CouponCreateValidation() 
        {
            RuleFor(coupon => coupon.Name).NotEmpty();
            RuleFor(coupon => coupon.Percent).InclusiveBetween(1, 100);
        }
    }
}

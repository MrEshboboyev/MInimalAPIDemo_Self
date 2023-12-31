using FluentValidation;
using MagicVillaCouponAPI_Self.Models.DTO;

namespace MagicVillaCouponAPI_Self.Validations
{
    public class CouponUpdateValidation : AbstractValidator<CouponUpdateDTO>
    {
        public CouponUpdateValidation() 
        {
            RuleFor(coupon => coupon.Id).NotEmpty().GreaterThan(0);
            RuleFor(coupon => coupon.Name).NotEmpty();
            RuleFor(coupon => coupon.Percent).InclusiveBetween(1, 100);
        }
    }
}

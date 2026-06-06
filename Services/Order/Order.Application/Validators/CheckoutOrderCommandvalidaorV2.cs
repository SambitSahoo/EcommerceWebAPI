using FluentValidation;
using Order.Application.Commands;

namespace Order.Application.Validators
{ 
    public class CheckoutOrderCommandvalidaorV2 : AbstractValidator<CheckoutOrderCommandV2>
    {
        public CheckoutOrderCommandvalidaorV2()
        {
            RuleFor(o => o.UserName)
                .NotEmpty().WithMessage("{UserName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{UserName} must not exceed 100 characters.");

            RuleFor(o => o.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .NotNull()
                .GreaterThan(-1).WithMessage("{TotalPrice} must be greater than zero.");
        }
    }
}
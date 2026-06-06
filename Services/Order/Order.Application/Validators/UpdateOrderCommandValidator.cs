using FluentValidation;
using Order.Application.Commands;

namespace Order.Application.Validators
{
    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .NotNull()
                .WithMessage("Id is required.")
                .GreaterThan(-1).WithMessage("Id cannot be negative.");
            RuleFor(o => o.UserName)
                .NotEmpty().WithMessage("{UserName} is required.")
                .NotNull()
                .MaximumLength(100).WithMessage("{UserName} must not exceed 100 characters.");
            RuleFor(o => o.TotalPrice)
                .NotEmpty().WithMessage("{TotalPrice} is required.")
                .NotNull()
                .GreaterThan(-1).WithMessage("{TotalPrice} must be greater than zero.");
            RuleFor(o => o.EmailAddress)
                .NotEmpty().WithMessage("{EmailAddress} is required.")
                .NotNull()
                .EmailAddress(FluentValidation.Validators.EmailValidationMode.AspNetCoreCompatible).WithMessage("{EmailAddress} is not a valid email address.")
                .MaximumLength(100).WithMessage("{EmailAddress} must not exceed 100 characters.");
            RuleFor(o => o.FirstName)
                .NotEmpty().WithMessage("{FirstName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{FirstName} must not exceed 50 characters");
            RuleFor(o => o.LastName)
                .NotEmpty().WithMessage("{LastName} is required.")
                .NotNull()
                .MaximumLength(50).WithMessage("{LastName} must not exceed 50 characters");
        }
    }

    // Placeholder for UpdateOrderCommand. Replace with your actual command class.

}
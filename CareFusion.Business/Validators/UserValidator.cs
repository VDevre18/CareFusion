using CareFusion.Model.Dtos;
using FluentValidation;

namespace CareFusion.Business.Validators;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100)
            .WithMessage("Username is required and must be less than 100 characters");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(256)
            .WithMessage("Valid email address is required and must be less than 256 characters");

        RuleFor(x => x.Role)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Role is required and must be less than 50 characters");

        RuleFor(x => x.FirstName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.FirstName))
            .WithMessage("First name must be less than 100 characters");

        RuleFor(x => x.LastName)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.LastName))
            .WithMessage("Last name must be less than 100 characters");
    }
}
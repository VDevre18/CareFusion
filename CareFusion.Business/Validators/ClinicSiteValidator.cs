using CareFusion.Model.Dtos;
using FluentValidation;

namespace CareFusion.Business.Validators;

public class ClinicSiteValidator : AbstractValidator<ClinicSiteDto>
{
    public ClinicSiteValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .WithMessage("Clinic site name is required and must be less than 200 characters");

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50)
            .WithMessage("Clinic site code is required and must be less than 50 characters");

        RuleFor(x => x.Address)
            .MaximumLength(200)
            .When(x => !string.IsNullOrEmpty(x.Address))
            .WithMessage("Address must be less than 200 characters");

        RuleFor(x => x.City)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.City))
            .WithMessage("City must be less than 100 characters");

        RuleFor(x => x.State)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.State))
            .WithMessage("State must be less than 100 characters");

        RuleFor(x => x.PostalCode)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.PostalCode))
            .WithMessage("Postal code must be less than 20 characters");

        RuleFor(x => x.Country)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.Country))
            .WithMessage("Country must be less than 100 characters");

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .When(x => !string.IsNullOrEmpty(x.Phone))
            .WithMessage("Phone number must be less than 20 characters");

        RuleFor(x => x.Email)
            .EmailAddress()
            .MaximumLength(256)
            .When(x => !string.IsNullOrEmpty(x.Email))
            .WithMessage("Valid email address is required and must be less than 256 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description must be less than 500 characters");
    }
}
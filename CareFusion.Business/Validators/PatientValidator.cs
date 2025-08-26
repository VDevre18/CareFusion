// Placeholder for Validators/PatientValidator.cs
using CareFusion.Model.Dtos;
using FluentValidation;

namespace CareFusion.Business.Validators;

public class PatientValidator : AbstractValidator<PatientDto>
{
    public PatientValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(100);
        RuleFor(x => x.MRN).MaximumLength(50);
        RuleFor(x => x.Gender).MaximumLength(25);
        RuleFor(x => x.DateOfBirth)
            .LessThan(DateTime.UtcNow)
            .WithMessage("Date of Birth cannot be in the future");
    }
}

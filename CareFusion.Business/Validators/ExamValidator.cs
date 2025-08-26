// Placeholder for Validators/ExamValidator.cs
using CareFusion.Model.Dtos;
using FluentValidation;

namespace CareFusion.Business.Validators;

public class ExamValidator : AbstractValidator<ExamDto>
{
    public ExamValidator()
    {
        RuleFor(x => x.PatientId).NotEmpty();
        RuleFor(x => x.Modality).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StudyType).NotEmpty().MaximumLength(100);
        RuleFor(x => x.StudyDateUtc).LessThanOrEqualTo(DateTime.UtcNow);
        RuleFor(x => x.Status).MaximumLength(50);
    }
}

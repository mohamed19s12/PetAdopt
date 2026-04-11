using FluentValidation;
using PetAdopt.Application.DTOs.Pet;

public class UpdatePetDtoValidator : AbstractValidator<UpdatePetDto>
{
    public UpdatePetDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Pet name is required")
            .MaximumLength(50).WithMessage("Name must not exceed 50 characters");

        RuleFor(x => x.Age)
            .GreaterThan(0).WithMessage("Age must be greater than 0")
            .LessThan(30).WithMessage("Age must be less than 30");

        RuleFor(x => x.Gender)
            .NotEmpty().WithMessage("Gender is required")
            .Must(g => g == "Male" || g == "Female")
            .WithMessage("Gender must be Male or Female");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required");

        RuleFor(x => x.HealthStatus)
            .NotEmpty().WithMessage("Health status is required");
    }
}
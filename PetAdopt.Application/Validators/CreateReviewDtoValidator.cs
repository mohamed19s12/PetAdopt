using FluentValidation;
using PetAdopt.Application.DTOs.Review;

namespace PetAdopt.Application.Validators
{
    public class CreateReviewDtoValidator : AbstractValidator<CreateReviewDto>
    {
        public CreateReviewDtoValidator()
        {
            RuleFor(x => x.TargetUserId)
                .NotEmpty().WithMessage("Target user is required");

            RuleFor(x => x.PetId)
                .GreaterThan(0).WithMessage("Pet is required");

            RuleFor(x => x.Rating)
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required")
                .MaximumLength(500).WithMessage("Comment must not exceed 500 characters");
        }
    }
}
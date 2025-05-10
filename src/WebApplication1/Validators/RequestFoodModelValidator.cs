using FluentValidation;
using WebApplication1.Models;

public class RequestFoodModelValidator : AbstractValidator<FoodRequestModel>
{
    public RequestFoodModelValidator()
    {
        RuleFor(f => f.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name can't exceed 100 characters");

        RuleFor(f => f.Description)
            .MaximumLength(500).WithMessage("Description can't exceed 500 characters");

        RuleFor(f => f.PhotoUrl)
            .NotEmpty().WithMessage("PhotoUrl is required")
            .Must(url => Uri.TryCreate(url, UriKind.Absolute, out _)).WithMessage("PhotoUrl must be a valid URL");

        RuleFor(f => f.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");

        RuleFor(f => f.PreparationTime)
            .GreaterThan(TimeSpan.Zero).WithMessage("Preparation time must be greater than 0");

        RuleFor(f => f.Recipe)
            .NotNull().WithMessage("Recipe is required")
            .Must(r => r.Count > 0).WithMessage("Recipe must contain at least one step");

        RuleFor(f => f.CategoryId)
            .NotEmpty().WithMessage("CategoryId is required");
    }
}
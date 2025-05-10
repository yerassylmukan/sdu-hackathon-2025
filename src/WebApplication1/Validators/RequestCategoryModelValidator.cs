using FluentValidation;
using WebApplication1.Models;

namespace WebApplication1.Validators;

public class RequestCategoryModelValidator : AbstractValidator<CategoryRequestModel>
{
    public RequestCategoryModelValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name can't exceed 100 characters");
    }
}
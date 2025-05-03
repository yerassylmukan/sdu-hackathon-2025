using FluentValidation;
using WebApplication1.Models;

namespace WebApplication1.Validators;

public class RequestBasketItemModelValidator : AbstractValidator<RequestBasketItemModel>
{
    public RequestBasketItemModelValidator()
    {
        RuleFor(x => x.FoodId).NotEmpty().WithMessage("FoodId is required.");
        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.");
    }
}
using FluentValidation;

namespace Api.Validators;

public class UpdatePriceRequestValidator : AbstractValidator<UpdatePriceRequest>
{
    public UpdatePriceRequestValidator()
    {
        RuleFor(request => request.ProductId).GreaterThan(0);
        RuleFor(request => request.NewPrice).GreaterThan(0);
    }
}
using FluentValidation;

namespace Api.Validators;

public class GetProductRequestValidator : AbstractValidator<GetProductRequest>
{
    public GetProductRequestValidator()
    {
        RuleFor(request => request.ProductId).GreaterThanOrEqualTo(0);
    }
}
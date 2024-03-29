using FluentValidation;

namespace Api.Validators;

public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
{
    public CreateProductRequestValidator()
    {
        RuleFor(request => request.Name).MinimumLength(3);
        RuleFor(request => request.Price).GreaterThan(0);
        RuleFor(request => request.WarehouseId).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Weight).GreaterThan(0);
        RuleFor(request => request.Type).Must(value => Enum.IsDefined(typeof(ProductType), value))
            .WithMessage("Type must be a valid ProductType value.");
    }
}
using FluentValidation;

namespace Api.Validators;

public class ListProductsRequestValidator : AbstractValidator<ListProductsRequest>
{
    public ListProductsRequestValidator()
    {
        RuleFor(request => request.Pagination.PageNumber).GreaterThanOrEqualTo(0);
        RuleFor(request => request.Pagination.PageSize).GreaterThanOrEqualTo(0);
        
        RuleFor(request => request.Filter.WarehouseId).GreaterThan(0);
        RuleFor(request => request.Filter.ProductType)
            .Must(value => Enum.IsDefined(typeof(ProductType), value))
            .WithMessage("Type must be a valid ProductType value.");

        RuleFor(request => request.Filter.StartDate.Year)
            .GreaterThan(2000).LessThanOrEqualTo(DateTime.Now.Year);
        RuleFor(request => request.Filter.StartDate.Month)
            .GreaterThan(0).LessThan(13);
        RuleFor(request => request.Filter.StartDate.Day)
            .GreaterThan(0).LessThanOrEqualTo(31);
    }
}
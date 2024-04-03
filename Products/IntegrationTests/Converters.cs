using Api;
using Domain.Models;

namespace IntegrationTests;

public static class Converters
{
    
    public static ListProductsRequest.Types.Pagination ConvertPaginationModelToPaginationRequestModel(Pagination pagination)
    {
        return new ListProductsRequest.Types.Pagination
        {
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize
        };
    }
    
    public static ListProductsRequest.Types.Filter ConvertFilterModelToFilterRequestModel(Filter filter)
    {
        return new ListProductsRequest.Types.Filter
        {
            ProductType = (Api.ProductType)filter.ProductType,
            StartDate = new Date
            {
                Day = filter.StartDate.Day,
                Month = filter.StartDate.Month,
                Year = filter.StartDate.Year
            },
            WarehouseId = filter.WarehouseId
        };
    }
}
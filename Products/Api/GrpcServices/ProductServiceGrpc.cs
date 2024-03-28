using Application;
using DataAccess.Models;
using Domain.Models;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;


namespace Api.GrpcServices;

public class ProductServiceGrpc: Api.ProductService.ProductServiceBase
{
    private readonly IProductService _productService;

    public ProductServiceGrpc(IProductService productService)
    {
        _productService = productService;
    }

    public override Task<CreateProductResponse> Create(CreateProductRequest request, ServerCallContext context)
    {
        var productCreation = new ProductCreation
        {
            Name = request.Name,
            Price = request.Price,
            Type = (Domain.Models.ProductType)request.Type,
            WarehouseId = request.WarehouseId,
            Weight = request.Weight
        };
        var productId = _productService.Create(productCreation);

        return Task.FromResult(new CreateProductResponse
        {
            ProductId = productId
        });
    }

    public override Task<UpdatePriceResponse> UpdatePrice(UpdatePriceRequest request, ServerCallContext context)
    {
        var oldPrice = _productService.UpdatePrice(request.ProductId, request.NewPrice);

        return Task.FromResult(new UpdatePriceResponse
        {
            OldPrice = oldPrice
        });
    }

    public override Task<GetProductResponse> Get(GetProductRequest request, ServerCallContext context)
    {
        var product = _productService.Get(request.ProductId);

        return Task.FromResult(new GetProductResponse
        {
            Product = MapToProtoProduct(product)
        });
    }

    public override Task<ListProductsResponse> List(ListProductsRequest request, ServerCallContext context)
    {
        var pagination = new Pagination
        {
            PageNumber = request.Pagination.PageNumber,
            PageSize = request.Pagination.PageSize
        };

        var filter = new Filter
        {
            ProductType = (Domain.Models.ProductType)request.Filter.ProductType,
            StartDate = new DateTime(
                request.Filter.StartDate.Year,
                request.Filter.StartDate.Month,
                request.Filter.StartDate.Day),
            WarehouseId = request.Filter.WarehouseId
        };
        
        var products = _productService.List(pagination, filter);

        var apiProtobufProducts = products.Select(product => MapToProtoProduct(product));
        var response = new ListProductsResponse();
        response.Products.AddRange(apiProtobufProducts);
        
        return Task.FromResult(response);
    }

    private static Api.Product MapToProtoProduct(Domain.Models.Product productModel)
        => new Product
        {
            CreationDate = new Date
            {
                Day = productModel.CreationDate.Day,
                Month = productModel.CreationDate.Month,
                Year = productModel.CreationDate.Year
            },
            Name = productModel.Name,
            Price = productModel.Price,
            ProductId = productModel.Id,
            ProductType = (ProductType)productModel.Type,
            WarehouseId = productModel.WarehouseId,
            Weight = productModel.Weight
        };
}
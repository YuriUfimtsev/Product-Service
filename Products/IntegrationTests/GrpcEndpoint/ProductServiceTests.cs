using Api;
using Domain.Models;
using FluentAssertions;
using UnitTests.Builders;
using Xunit.Abstractions;
using static IntegrationTests.Converters;
using ProductService = Api.ProductService;
using ProductType = Api.ProductType;

namespace IntegrationTests.GrpcEndpoint;

public class ProductServiceTests : IntegrationTestBase
{
    public ProductServiceTests(GrpcTestFixture<Startup> fixture, ITestOutputHelper outputHelper)
        : base(fixture, outputHelper)
    {
    }

    [Fact]
    public async Task Create_NewProductCreation_ShouldReturnProductId()
    {
        var client = new ProductService.ProductServiceClient(Channel);

        var productCreation = new ProductCreationBuilder().Build();
        var createProductRequest = new CreateProductRequest
        {
            Name = productCreation.Name,
            Price = productCreation.Price,
            Type = (ProductType)productCreation.Type,
            WarehouseId = productCreation.WarehouseId,
            Weight = productCreation.Weight
        };
        var expectedId = 0;
        
        Fixture.ProductRepositoryFake
            .Setup(fake => fake.Create(productCreation))
            .Returns(expectedId);

        var response = await client.CreateAsync(createProductRequest);

        response.ProductId.Should().Be(expectedId);
    }
    
    [Fact]
    public async Task Get_ExistingProduct_ShouldReturnProduct()
    {
        var client = new ProductService.ProductServiceClient(Channel);
        
        var productId = 0;
        var expectedProduct = new ProductBuilder().WithId(productId).Build();
        var getProductRequest = new GetProductRequest
        {
            ProductId = productId
        };
        
        Fixture.ProductRepositoryFake
            .Setup(fake => fake.Get(productId))
            .Returns(expectedProduct);

        var response = await client.GetAsync(getProductRequest);

        response.Product.ProductId.Should().Be(expectedProduct.Id);
    }
    
    [Fact]
    public async Task UpdatePrice_OfExistingProduct_ShouldReturnOldPrice()
    {
        var client = new ProductService.ProductServiceClient(Channel);
        
        var productId = 0;
        var newPrice = 100;
        var expectedOldPrice = 10;
        var updatePriceRequest = new UpdatePriceRequest
        {
            NewPrice = newPrice,
            ProductId = productId
        };
        
        Fixture.ProductRepositoryFake
            .Setup(fake => fake.UpdatePrice(productId, newPrice))
            .Returns(expectedOldPrice);

        var response = await client.UpdatePriceAsync(updatePriceRequest);
        
        response.OldPrice.Should().Be(expectedOldPrice);
    }

    [Fact]
    public async Task List_FromRepositoryWithThreeProductsWithCorrespondingFilterAndPagination_ShouldReturnThreeProducts()
    {
        var client = new ProductService.ProductServiceClient(Channel);
        
        var paginationContext = new Pagination
        {
            PageNumber = 0,
            PageSize = 4
        };
        var filter = new Filter
        {
            ProductType = Domain.Models.ProductType.General,
            StartDate = DateTime.MinValue,
            WarehouseId = 0
        };
        var expectedPage = new List<Domain.Models.Product> 
        { 
            new ProductBuilder().WithId(0).WithType(Domain.Models.ProductType.General).Build(),
            new ProductBuilder().WithId(1).WithType(Domain.Models.ProductType.General).Build(),
            new ProductBuilder().WithId(2).WithType(Domain.Models.ProductType.General).Build()
        };
        var listProductsRequest = new ListProductsRequest
        {
            Filter = ConvertFilterModelToFilterRequestModel(filter),
            Pagination = ConvertPaginationModelToPaginationRequestModel(paginationContext)
        };
        
        Fixture.ProductRepositoryFake
            .Setup(fake => fake.List(paginationContext, filter))
            .Returns(expectedPage);
        
        var response = await client.ListAsync(listProductsRequest);
        
        response.Products.Should().HaveCount(expectedPage.Count);
    }
}
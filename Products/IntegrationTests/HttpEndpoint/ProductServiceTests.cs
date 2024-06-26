using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Api;
using Domain.Models;
using FluentAssertions;
using UnitTests.Builders;
using static IntegrationTests.Converters;

namespace IntegrationTests.HttpEndpoint;

public class ProductServiceTests : IClassFixture<MyWebApplicationFactory<Startup>>
{
    private readonly MyWebApplicationFactory<Startup> _webApplicationFactory;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ProductServiceTests(MyWebApplicationFactory<Startup> webApplicationFactory)
    {
        _webApplicationFactory = webApplicationFactory;
    }

    [Fact]
    public async Task Create_NewProductCreation_ShouldReturnProductId()
    {
        var client = _webApplicationFactory.CreateClient();

        var productCreation = new ProductCreationBuilder().Build();
        var expectedId = 1;

        _webApplicationFactory.ProductRepositoryFake
            .Setup(fake => fake.Create(productCreation))
            .Returns(expectedId);

        var response = await client.PostAsJsonAsync(
            "/product/create", productCreation, _jsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var actualId = CreateProductResponse.Parser.ParseJson(responseContent).ProductId;
        actualId.Should().Be(expectedId);
    }

    [Fact]
    public async Task Get_ExistingProduct_ShouldReturnProduct()
    {
        var client = _webApplicationFactory.CreateClient();

        var productId = 1;
        var expectedProduct = new ProductBuilder().WithId(productId).Build();

        _webApplicationFactory.ProductRepositoryFake
            .Setup(fake => fake.Get(productId))
            .Returns(expectedProduct);

        var response = await client.GetAsync($"/product/get?productId={productId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var actualProduct = GetProductResponse.Parser.ParseJson(responseContent).Product;
        actualProduct.ProductId.Should().Be(expectedProduct.Id);
    }

    [Fact]
    public async Task UpdatePrice_OfExistingProduct_ShouldReturnOldPrice()
    {
        var client = _webApplicationFactory.CreateClient();

        var productId = 1;
        var newPrice = 100;
        var expectedOldPrice = 10;

        _webApplicationFactory.ProductRepositoryFake
            .Setup(fake => fake.UpdatePrice(productId, newPrice))
            .Returns(expectedOldPrice);

        var response = await client.GetAsync(
            $"/product/updatePrice?productId={productId}&newPrice={newPrice}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var actualOldPrice = UpdatePriceResponse.Parser.ParseJson(responseContent).OldPrice;
        actualOldPrice.Should().Be(expectedOldPrice);
    }

    [Fact]
    public async Task List_FromRepositoryWithThreeProductsWithCorrespondingFilterAndPagination_ShouldReturnThreeProducts()
    {
        var client = _webApplicationFactory.CreateClient();

        var paginationContext = new Pagination
        {
            PageNumber = 0,
            PageSize = 4
        };
        var filter = new Filter
        {
            ProductType = Domain.Models.ProductType.General,
            StartDate = DateTime.MinValue,
            WarehouseId = 1
        };
        var expectedPage = new List<Domain.Models.Product>
        {
            new ProductBuilder().WithId(0).WithType(Domain.Models.ProductType.General).WithWarehouseId(1).Build(),
            new ProductBuilder().WithId(1).WithType(Domain.Models.ProductType.General).WithWarehouseId(1).Build(),
            new ProductBuilder().WithId(2).WithType(Domain.Models.ProductType.General).WithWarehouseId(1).Build()
        };
        var requestModel = new ListProductsRequest
        {
            Filter = ConvertFilterModelToFilterRequestModel(filter),
            Pagination = ConvertPaginationModelToPaginationRequestModel(paginationContext)
        };

        _webApplicationFactory.ProductRepositoryFake
            .Setup(fake => fake.List(paginationContext, filter))
            .Returns(expectedPage);

        var response = await client.PostAsJsonAsync("/product/list", requestModel, _jsonSerializerOptions);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var responseContent = await response.Content.ReadAsStringAsync();
        var actualPage = ListProductsResponse.Parser.ParseJson(responseContent).Products;
        actualPage.Should().HaveCount(expectedPage.Count);
    }
}
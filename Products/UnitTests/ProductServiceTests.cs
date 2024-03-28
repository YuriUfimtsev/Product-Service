using Application;
using Bogus;
using Domain;
using Domain.Models;
using Moq;
using UnitTests.Builders;

namespace UnitTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryFake = new(MockBehavior.Strict);
    private readonly IProductService _productService;

    public ProductServiceTests()
    {
        _productService = new ProductService(_productRepositoryFake.Object);
    }

    [Fact]
    public void Create_NewProductCreation_ShouldReturnProductId()
    {
        var productCreation = new ProductCreationBuilder().Build();
        var expectedId = 0;

        _productRepositoryFake
            .Setup(fake => fake.Create(productCreation))
            .Returns(expectedId);

        var actualId = _productService.Create(productCreation);
        
        Assert.Equal(expectedId, actualId);
        _productRepositoryFake.Verify(fake => fake.Create(productCreation), Times.Once);
    }

    [Fact]
    public void Get_ExistingProduct_ShouldReturnProduct()
    {
        var existingProductId = 0;
        var expectedProduct = new ProductBuilder().Build();

        _productRepositoryFake
            .Setup(fake => fake.Get(existingProductId))
            .Returns(expectedProduct);

        var actualProduct = _productService.Get(existingProductId);
        
        Assert.Equivalent(expectedProduct, actualProduct);
        _productRepositoryFake.Verify(fake => fake.Get(existingProductId), Times.Once);
    }

    [Fact]
    public void UpdatePrice_OfExistingProduct_ShouldReturnOldPrice()
    {
        var existingProductId = 0;
        var newPrice = 100;
        var expectedOldPrice = 10;
        
        _productRepositoryFake
            .Setup(fake => fake.UpdatePrice(existingProductId, newPrice))
            .Returns(expectedOldPrice);

        var actualOldPrice = _productService.UpdatePrice(existingProductId, newPrice);
        
        Assert.Equal(expectedOldPrice, actualOldPrice);
        _productRepositoryFake
            .Verify(fake => fake.UpdatePrice(existingProductId, newPrice), Times.Once);
    }

    [Fact]
    public void List_FromRepositoryWithTwoProductsWithCorrespondingFilterAndPagination_ShouldReturnTwoProducts()
    {
        var productsWarehouseId = 0;
        var productsType = ProductType.General;
        var firstExpectedProduct =
            new ProductBuilder().WithType(productsType).WithWarehouseId(productsWarehouseId).Build();
        var secondExpectedProduct = 
            new ProductBuilder().WithType(productsType).WithWarehouseId(productsWarehouseId).Build();
        var expectedList = new List<Product> { firstExpectedProduct, secondExpectedProduct };

        var paginationContext = new Faker<Pagination>()
            .RuleFor(faker => faker.PageNumber, 0)
            .RuleFor(faker => faker.PageSize, 5)
            .Generate();
        var filter = new Faker<Filter>()
            .RuleFor(faker => faker.ProductType, productsType)
            .RuleFor(faker => faker.WarehouseId, productsWarehouseId)
            .RuleFor(faker => faker.StartDate, DateTime.MinValue);
        
        _productRepositoryFake
            .Setup(fake => fake.List(paginationContext, filter))
            .Returns(expectedList);

        var actualList = _productService.List(paginationContext, filter);

        Assert.Contains(firstExpectedProduct, actualList);
        Assert.Contains(secondExpectedProduct, actualList);
        Assert.Equal(2, actualList.Count);
        _productRepositoryFake
            .Verify(fake => fake.List(paginationContext, filter), Times.Once);
    }
}
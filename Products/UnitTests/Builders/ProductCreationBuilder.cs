using Bogus;
using Domain.Models;

namespace UnitTests.Builders;

public class ProductCreationBuilder
{
    private string? _name;
    private int? _price;
    private int? _weight;
    private ProductType? _type;
    private int? _warehouseId;

    public ProductCreationBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductCreationBuilder WithPrice(int price)
    {
        _price = price;
        return this;
    }

    public ProductCreationBuilder WithWeight(int weight)
    {
        _weight = weight;
        return this;
    }

    public ProductCreationBuilder WithType(ProductType type)
    {
        _type = type;
        return this;
    }

    public ProductCreationBuilder WithWarehouseId(int warehouseId)
    {
        _warehouseId = warehouseId;
        return this;
    }

    public ProductCreation Build()
    {
        var faker = new Faker();
        return new ProductCreation
        {
            Name = _name ?? faker.Name.FullName(),
            Price = _price ?? faker.Random.Int(0),
            Weight = _weight ?? faker.Random.Int(0),
            Type = _type ?? ProductType.General,
            WarehouseId = _warehouseId ?? faker.Random.Int(1)
        };
    }
}
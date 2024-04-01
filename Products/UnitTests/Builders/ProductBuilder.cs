using Bogus;
using Domain.Models;

namespace UnitTests.Builders;

public class ProductBuilder
{
    private int? _id;
    private DateTime? _creationDate;
    private string? _name;
    private int? _price;
    private int? _weight;
    private ProductType? _type;
    private int? _warehouseId;

    public ProductBuilder WithId(int id)
    {
        _id = id;
        return this;
    }

    public ProductBuilder WithCreationDate(DateTime creationDate)
    {
        _creationDate = creationDate;
        return this;
    }

    public ProductBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ProductBuilder WithPrice(int price)
    {
        _price = price;
        return this;
    }

    public ProductBuilder WithWeight(int weight)
    {
        _weight = weight;
        return this;
    }

    public ProductBuilder WithType(ProductType type)
    {
        _type = type;
        return this;
    }

    public ProductBuilder WithWarehouseId(int warehouseId)
    {
        _warehouseId = warehouseId;
        return this;
    }

    public Product Build()
    {
        var faker = new Faker();
        var productCreation = new ProductCreationBuilder().Build();
        return new Product
        {
            Id = _id ?? faker.Random.Int(1),
            CreationDate = _creationDate ?? DateTime.Now,
            Name = _name ?? productCreation.Name,
            Price = _price ?? productCreation.Price,
            Weight = _weight ?? productCreation.Weight,
            Type = _type ?? productCreation.Type,
            WarehouseId = _warehouseId ?? productCreation.WarehouseId
        };
    }
}
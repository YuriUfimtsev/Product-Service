using System.Collections.Concurrent;
using DataAccess.Models;
using Domain;
using Domain.Exceptions;
using Domain.Models;

namespace DataAccess;

public class ProductRepository : IProductRepository
{
    private readonly ConcurrentDictionary<int, Product> _store = new();
    private volatile int _lastId = 0;
    
    public int Create(ProductCreation productModel)
    {
        var id = Interlocked.Increment(ref _lastId);
        var product = new Product
        {
            Id = id,
            Name = productModel.Name,
            Price = productModel.Price,
            Weight = productModel.Weight,
            Type = productModel.Type,
            CreationDate = DateTime.Now,
            WarehouseId = productModel.WarehouseId
        };

        if (!_store.TryAdd(id, product))
        {
            throw new ProductAlreadyExistsException(
                $"Product with {product.Id} id has already been added.");
        }

        return id;
    }

    public Product Get(int productId)
    {
        if (!_store.TryGetValue(productId, out var product))
        {
            throw new ProductNotFoundException($"Product {productId} hasn't been found.");
        }

        return product;
    }

    public int UpdatePrice(int productId, int newPrice)
    {
        if (!_store.ContainsKey(productId))
        {
            throw new ProductNotFoundException($"Product {productId} hasn't been found.");
        }

        var oldPrice = _store[productId].Price;
        _store[productId].Price = newPrice;
        return oldPrice;
    }

    public List<Product> List(Pagination paginationContext, Filter filter)
    {
        var products = _store.Values
            .Where(product => product.Type == filter.ProductType)
            .Where(product => product.WarehouseId == filter.WarehouseId)
            .Where(product => product.CreationDate >= filter.StartDate);

        return products
            .Skip((paginationContext.PageNumber - 1) * paginationContext.PageSize)
            .Take(paginationContext.PageSize)
            .ToList();
    }
}
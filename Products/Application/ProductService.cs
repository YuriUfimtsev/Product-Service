using DataAccess.Models;
using Domain;
using Domain.Exceptions;
using Domain.Models;
using ApplicationException = Application.Exceptions.ApplicationException;

namespace Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public int Create(ProductCreation newProduct)
    { 
        return _productRepository.Create(newProduct);
    }

    public Product Get(int productId)
    { 
        return _productRepository.Get(productId);
    }

    public int UpdatePrice(int productId, int newPrice)
    { 
        return _productRepository.UpdatePrice(productId, newPrice);
    }

    public List<Product> List(Pagination paginationContext, Filter filter)
    {
        return _productRepository.List(paginationContext, filter);
    }
}
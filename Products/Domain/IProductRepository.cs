using Domain.Models;

namespace Domain;

public interface IProductRepository
{
    public int Create(ProductCreation productModel);
    
    public Product Get(int productId);
    
    public int UpdatePrice(int productId, int newPrice);
    
    public List<Product> List(Pagination paginationContext, Filter filter);
}
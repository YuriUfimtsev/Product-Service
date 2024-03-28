using DataAccess.Models;
using Domain.Models;

namespace Application;

public interface IProductService
{
    public int Create(ProductCreation newProduct);
    
    public Product Get(int productId);
    
    public int UpdatePrice(int productId, int newPrice);
    
    public List<Product> List(Pagination paginationContext, Filter filter);
}
namespace Domain.Models;

public class Product
{
    public int Id { get; init; }
    
    public string Name { get; init; }
    
    public int Price { get; set; }
    
    public int Weight { get; init; }
    
    public ProductType Type { get; init; }
    
    public DateTime CreationDate { get; init; }
    
    public int WarehouseId { get; set; }
}
namespace Domain.Models;

public record ProductCreation
{
    public string Name { get; init; }
    
    public int Price { get; init; }
    
    public int Weight { get; init; }
    
    public ProductType Type { get; init; }
    
    public int WarehouseId { get; init; }
}
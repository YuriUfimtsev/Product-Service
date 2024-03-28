namespace Domain.Models;

public record Filter
{
    public DateTime StartDate { get; init; }
    
    public ProductType ProductType { get; init; }
    
    public int WarehouseId { get; init; }
}
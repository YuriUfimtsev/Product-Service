namespace Domain.Exceptions;

public class ProductNotFoundException : Exception
{
    public ProductNotFoundException()
    {
    }

    public ProductNotFoundException(string message) : base(message)
    {
    }
}
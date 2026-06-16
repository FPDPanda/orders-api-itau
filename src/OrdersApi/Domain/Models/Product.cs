namespace OrdersApi.Domain.Models;

public class Product
{
    public Guid Id { get; set; }
    public string ImageURL { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

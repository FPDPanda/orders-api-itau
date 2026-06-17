using OrdersApi.Domain.Models;

namespace OrdersApi.Dtos;

public record ProductResponse(
    Guid Id,
    string Name,
    string ImageURL,
    string Description,
    decimal Price)
{
    public static ProductResponse From(Product product) => new(
        product.Id,
        product.Name,
        product.ImageURL,
        product.Description,
        product.Price);
}

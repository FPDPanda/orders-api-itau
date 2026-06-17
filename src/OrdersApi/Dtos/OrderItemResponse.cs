using OrdersApi.Domain.Models;

namespace OrdersApi.Dtos;

public record OrderItemResponse(
    Guid ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity)
{
    public static OrderItemResponse From(OrderItem item) => new(
        item.ProductId,
        item.Product.Name,
        item.UnitPrice,
        item.Quantity);
}

using OrdersApi.Domain.Models;

namespace OrdersApi.Dtos;

public record OrderResponse(
    Guid Id,
    DateTime CreationDateTime,
    string Type,
    decimal OriginalValue,
    decimal DebitedValue,
    IReadOnlyList<OrderItemResponse> Items,
    string User,
    string Status,
    string TrackingURL)
{
    public static OrderResponse From(Order order) => new(
        order.Id,
        order.CreationDateTime,
        order.Type.ToString(),
        order.OriginalValue,
        order.DebitedValue,
        order.Items.Select(OrderItemResponse.From).ToList(),
        order.User,
        order.Status.ToString(),
        order.TrackingURL);
}

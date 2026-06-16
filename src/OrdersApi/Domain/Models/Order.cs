using OrdersApi.Domain.Enums;

namespace OrdersApi.Domain.Models;

public class Order
{
    public Guid Id { get; set; }
    public DateTime CreationDateTime { get; set; }
    public OrderType Type { get; set; }
    public decimal OriginalValue { get; set; }
    public decimal DebitedValue { get; set; }
    public List<Product> Products { get; set; } = new();
    public string User { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public string TrackingURL { get; set; } = string.Empty;

    public bool CanTransitionTo(OrderStatus newStatus) => (Status, newStatus) switch
    {
        (OrderStatus.New, OrderStatus.Confirmed) => true,
        (OrderStatus.Confirmed, OrderStatus.Shipped) => true,
        (OrderStatus.Shipped, OrderStatus.Completed) => true,
        _ => false
    };

    public bool CanModifyItems() => Status == OrderStatus.New;
}

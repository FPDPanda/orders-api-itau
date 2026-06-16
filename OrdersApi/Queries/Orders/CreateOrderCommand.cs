using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Orders;

public record CreateOrderCommand(
    OrdersApi.Domain.Enums.OrderType Type,
    decimal OriginalValue,
    decimal DebitedValue,
    List<Guid> Products,
    string User,
    string TrackingURL) : IRequest<Order>;

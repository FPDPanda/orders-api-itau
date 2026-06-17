using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Commands.Orders;

public record CreateOrderCommand(
    OrdersApi.Domain.Enums.OrderType Type,
    IReadOnlyList<Guid> ProductIds,
    string User,
    string TrackingURL) : IRequest<Order>;

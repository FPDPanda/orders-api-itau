using MediatR;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Orders;

public record UpdateOrderStatusCommand(Guid OrderId, OrderStatus Status) : IRequest<Order?>;

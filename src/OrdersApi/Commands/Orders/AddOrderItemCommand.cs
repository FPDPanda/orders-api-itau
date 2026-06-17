using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Commands.Orders;

public record AddOrderItemCommand(Guid OrderId, Guid ItemId) : IRequest<Order?>;

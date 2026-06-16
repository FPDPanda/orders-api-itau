using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Orders;

public record AddOrderItemCommand(Guid OrderId, Guid ItemId) : IRequest<Order?>;

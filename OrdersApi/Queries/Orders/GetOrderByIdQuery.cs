using MediatR;
using OrdersApi.Domain.Models;

namespace OrdersApi.Queries.Orders;

public record GetOrderByIdQuery(Guid OrderId) : IRequest<Order?>;

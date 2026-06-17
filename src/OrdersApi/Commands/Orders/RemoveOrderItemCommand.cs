using MediatR;

namespace OrdersApi.Commands.Orders;

public record RemoveOrderItemCommand(Guid OrderId, Guid ItemId) : IRequest<bool>;

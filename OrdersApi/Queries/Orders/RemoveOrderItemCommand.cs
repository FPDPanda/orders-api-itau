using MediatR;

namespace OrdersApi.Queries.Orders;

public record RemoveOrderItemCommand(Guid OrderId, Guid ItemId) : IRequest<bool>;

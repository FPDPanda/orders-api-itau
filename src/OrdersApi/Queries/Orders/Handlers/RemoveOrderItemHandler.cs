using MediatR;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Orders.Handlers;

public class RemoveOrderItemHandler : IRequestHandler<RemoveOrderItemCommand, bool>
{
    private readonly IOrderRepository _repository;

    public RemoveOrderItemHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(RemoveOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId);
        if (order is null)
            return false;

        if (!order.CanModifyItems())
            throw new InvalidOperationException($"Cannot modify items on an order with status {order.Status}.");

        return await _repository.RemoveItemAsync(request.OrderId, request.ItemId);
    }
}

using MediatR;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Commands.Orders.Handlers;

public class AddOrderItemHandler : IRequestHandler<AddOrderItemCommand, Order?>
{
    private readonly IOrderRepository _repository;

    public AddOrderItemHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order?> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        var order = await _repository.GetByIdAsync(request.OrderId);
        if (order is null)
            return null;

        if (!order.CanModifyItems())
            throw new InvalidOperationException($"Cannot modify items on an order with status {order.Status}.");

        return await _repository.AddItemAsync(request.OrderId, request.ItemId);
    }
}

using MediatR;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Orders.Handlers;

public class AddOrderItemHandler : IRequestHandler<AddOrderItemCommand, Order?>
{
    private readonly IOrderRepository _repository;

    public AddOrderItemHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order?> Handle(AddOrderItemCommand request, CancellationToken cancellationToken)
    {
        return await _repository.AddItemAsync(request.OrderId, request.ItemId);
    }
}

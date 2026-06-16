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
        return await _repository.RemoveItemAsync(request.OrderId, request.ItemId);
    }
}

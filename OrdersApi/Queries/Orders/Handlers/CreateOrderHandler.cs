using MediatR;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _repository;

    public CreateOrderHandler(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            Type = request.Type,
            OriginalValue = request.OriginalValue,
            DebitedValue = request.DebitedValue,
            User = request.User,
            Status = OrderStatus.New,
            TrackingURL = request.TrackingURL
        };

        return await _repository.CreateAsync(order);
    }
}

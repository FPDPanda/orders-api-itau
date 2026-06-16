using MediatR;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderHandler(IOrderRepository orderRepository, IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetByIdsAsync(request.ProductIds);

        var missingIds = request.ProductIds.Except(products.Select(p => p.Id)).ToList();
        if (missingIds.Count > 0)
            throw new ArgumentException($"Products not found: {string.Join(", ", missingIds)}");

        var totalValue = products.Sum(p => p.Price);

        var order = new Order
        {
            Type = request.Type,
            Products = products,
            OriginalValue = totalValue,
            DebitedValue = totalValue,
            User = request.User,
            Status = OrderStatus.New,
            TrackingURL = request.TrackingURL
        };

        return await _orderRepository.CreateAsync(order);
    }
}

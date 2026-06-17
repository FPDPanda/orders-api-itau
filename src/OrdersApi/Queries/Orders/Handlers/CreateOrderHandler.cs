using MediatR;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Queries.Orders.Handlers;

public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IDiscountRepository _discountRepository;

    public CreateOrderHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IDiscountRepository discountRepository)
    {
        _orderRepository    = orderRepository;
        _productRepository  = productRepository;
        _discountRepository = discountRepository;
    }

    public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var distinctIds = request.ProductIds.Distinct().ToList();
        var products    = await _productRepository.GetByIdsAsync(distinctIds);

        var missingIds = distinctIds.Except(products.Select(p => p.Id)).ToList();
        if (missingIds.Count > 0)
            throw new ArgumentException($"Products not found: {string.Join(", ", missingIds)}");

        var quantityById = request.ProductIds
            .GroupBy(id => id)
            .ToDictionary(g => g.Key, g => g.Count());

        var productById = products.ToDictionary(p => p.Id);

        var items = quantityById.Select(kv => new OrderItem
        {
            ProductId = kv.Key,
            Product   = productById[kv.Key],
            Quantity  = kv.Value
        }).ToList();

        var originalValue = items.Sum(i => i.Product.Price * i.Quantity);
        var discount      = await _discountRepository.GetActiveByOrderTypeAsync(request.Type);
        var debitedValue  = discount?.Apply(originalValue) ?? originalValue;

        var order = new Order
        {
            Type          = request.Type,
            Items         = items,
            OriginalValue = originalValue,
            DebitedValue  = debitedValue,
            User          = request.User,
            Status        = OrderStatus.New,
            TrackingURL   = request.TrackingURL
        };

        return await _orderRepository.CreateAsync(order);
    }
}

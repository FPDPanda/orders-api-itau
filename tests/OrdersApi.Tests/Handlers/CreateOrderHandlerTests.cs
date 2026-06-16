using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Orders;
using OrdersApi.Queries.Orders.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class CreateOrderHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_orderRepositoryMock.Object, _productRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderWithStatusNew()
    {
        var product = new Product { Id = Guid.NewGuid(), Price = 50m };
        var command = new CreateOrderCommand(OrderType.Standard, [product.Id], "user@test.com", "https://tracking.com");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds)).ReturnsAsync([product]);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderStatus.New, result.Status);
    }

    [Fact]
    public async Task Handle_ShouldCalculateValuesFromProductPrices()
    {
        var p1 = new Product { Id = Guid.NewGuid(), Price = 100m };
        var p2 = new Product { Id = Guid.NewGuid(), Price = 49.90m };
        var command = new CreateOrderCommand(OrderType.Express, [p1.Id, p2.Id], "user@test.com", "");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds)).ReturnsAsync([p1, p2]);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(149.90m, result.OriginalValue);
        Assert.Equal(149.90m, result.DebitedValue);
    }

    [Fact]
    public async Task Handle_ShouldLinkProductsToOrder()
    {
        var product = new Product { Id = Guid.NewGuid(), Price = 30m };
        var command = new CreateOrderCommand(OrderType.Standard, [product.Id], "user@test.com", "");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds)).ReturnsAsync([product]);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Single(result.Products);
        Assert.Equal(product.Id, result.Products[0].Id);
    }

    [Fact]
    public async Task Handle_ShouldMapRemainingCommandFieldsToOrder()
    {
        var product = new Product { Id = Guid.NewGuid(), Price = 10m };
        var command = new CreateOrderCommand(OrderType.Subscription, [product.Id], "user@test.com", "https://tracking.com");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds)).ReturnsAsync([product]);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderType.Subscription, result.Type);
        Assert.Equal("user@test.com", result.User);
        Assert.Equal("https://tracking.com", result.TrackingURL);
    }

    [Fact]
    public async Task Handle_ShouldThrow_WhenAnyProductIdDoesNotExist()
    {
        var knownId = Guid.NewGuid();
        var unknownId = Guid.NewGuid();
        var command = new CreateOrderCommand(OrderType.Standard, [knownId, unknownId], "user@test.com", "");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds))
            .ReturnsAsync([new Product { Id = knownId, Price = 10m }]);

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallOrderRepositoryCreateOnce()
    {
        var product = new Product { Id = Guid.NewGuid(), Price = 10m };
        var command = new CreateOrderCommand(OrderType.Standard, [product.Id], "user@test.com", "");

        _productRepositoryMock.Setup(r => r.GetByIdsAsync(command.ProductIds)).ReturnsAsync([product]);
        _orderRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Order>())).ReturnsAsync(new Order());

        await _handler.Handle(command, CancellationToken.None);

        _orderRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }
}

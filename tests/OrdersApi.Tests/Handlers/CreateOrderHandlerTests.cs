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
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly CreateOrderHandler _handler;

    public CreateOrderHandlerTests()
    {
        _handler = new CreateOrderHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderWithStatusNew()
    {
        var command = new CreateOrderCommand(OrderType.Standard, 100m, 90m, "user@test.com", "https://tracking.com");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderStatus.New, result.Status);
    }

    [Fact]
    public async Task Handle_ShouldMapAllCommandFieldsToOrder()
    {
        var command = new CreateOrderCommand(OrderType.Express, 200m, 180m, "user@test.com", "https://tracking.com");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync((Order o) => o);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderType.Express, result.Type);
        Assert.Equal(200m, result.OriginalValue);
        Assert.Equal(180m, result.DebitedValue);
        Assert.Equal("user@test.com", result.User);
        Assert.Equal("https://tracking.com", result.TrackingURL);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryCreateExactlyOnce()
    {
        var command = new CreateOrderCommand(OrderType.Standard, 100m, 100m, "user@test.com", "");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync(new Order());

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Order>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrderReturnedByRepository()
    {
        var expected = new Order { Id = Guid.NewGuid(), User = "repo-user" };
        var command = new CreateOrderCommand(OrderType.Subscription, 50m, 50m, "user@test.com", "");

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Order>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(expected.Id, result.Id);
    }
}

using Moq;
using Xunit;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Orders;
using OrdersApi.Queries.Orders.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class AddOrderItemHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly AddOrderItemHandler _handler;

    public AddOrderItemHandlerTests()
    {
        _handler = new AddOrderItemHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdatedOrder_WhenItemAdded()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var expected = new Order { Id = orderId };

        _repositoryMock
            .Setup(r => r.AddItemAsync(orderId, itemId))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.AddItemAsync(orderId, itemId))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdsToRepository()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.AddItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        _repositoryMock.Verify(r => r.AddItemAsync(orderId, itemId), Times.Once);
    }
}

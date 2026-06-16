using Moq;
using Xunit;
using OrdersApi.Queries.Orders;
using OrdersApi.Queries.Orders.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class RemoveOrderItemHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly RemoveOrderItemHandler _handler;

    public RemoveOrderItemHandlerTests()
    {
        _handler = new RemoveOrderItemHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenItemRemoved()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.RemoveItemAsync(orderId, itemId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new RemoveOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderOrItemDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.RemoveItemAsync(orderId, itemId))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new RemoveOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdsToRepository()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.RemoveItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await _handler.Handle(new RemoveOrderItemCommand(orderId, itemId), CancellationToken.None);

        _repositoryMock.Verify(r => r.RemoveItemAsync(orderId, itemId), Times.Once);
    }
}

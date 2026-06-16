using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
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
    public async Task Handle_ShouldReturnTrue_WhenOrderIsNewAndItemRemoved()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.New };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _repositoryMock.Setup(r => r.RemoveItemAsync(orderId, itemId)).ReturnsAsync(true);

        var result = await _handler.Handle(new RemoveOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        var result = await _handler.Handle(new RemoveOrderItemCommand(orderId, Guid.NewGuid()), CancellationToken.None);

        Assert.False(result);
    }

    [Theory]
    [InlineData(OrderStatus.Confirmed)]
    [InlineData(OrderStatus.Shipped)]
    [InlineData(OrderStatus.Completed)]
    public async Task Handle_ShouldThrow_WhenOrderStatusIsNotNew(OrderStatus status)
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = status };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new RemoveOrderItemCommand(orderId, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallRemoveItemAsync_WhenOrderIsNew()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.New };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _repositoryMock.Setup(r => r.RemoveItemAsync(orderId, itemId)).ReturnsAsync(true);

        await _handler.Handle(new RemoveOrderItemCommand(orderId, itemId), CancellationToken.None);

        _repositoryMock.Verify(r => r.RemoveItemAsync(orderId, itemId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallRemoveItemAsync_WhenOrderStatusIsNotNew()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.Shipped };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new RemoveOrderItemCommand(orderId, Guid.NewGuid()), CancellationToken.None));

        _repositoryMock.Verify(r => r.RemoveItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }
}

using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Commands.Orders;
using OrdersApi.Commands.Orders.Handlers;
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
    public async Task Handle_ShouldReturnUpdatedOrder_WhenOrderIsNew()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.New };
        var updated = new Order { Id = orderId, Status = OrderStatus.New };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _repositoryMock.Setup(r => r.AddItemAsync(orderId, itemId)).ReturnsAsync(updated);

        var result = await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync((Order?)null);

        var result = await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        Assert.Null(result);
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
            _handler.Handle(new AddOrderItemCommand(orderId, Guid.NewGuid()), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallAddItemAsync_WhenOrderIsNew()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.New };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);
        _repositoryMock.Setup(r => r.AddItemAsync(orderId, itemId)).ReturnsAsync(new Order());

        await _handler.Handle(new AddOrderItemCommand(orderId, itemId), CancellationToken.None);

        _repositoryMock.Verify(r => r.AddItemAsync(orderId, itemId), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotCallAddItemAsync_WhenOrderStatusIsNotNew()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.Confirmed };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId)).ReturnsAsync(order);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _handler.Handle(new AddOrderItemCommand(orderId, Guid.NewGuid()), CancellationToken.None));

        _repositoryMock.Verify(r => r.AddItemAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }
}

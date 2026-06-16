using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Orders;
using OrdersApi.Queries.Orders.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class UpdateOrderStatusHandlerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock = new();
    private readonly UpdateOrderStatusHandler _handler;

    public UpdateOrderStatusHandlerTests()
    {
        _handler = new UpdateOrderStatusHandler(_orderRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdatedOrder_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.Confirmed };
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Confirmed);

        _orderRepositoryMock.Setup(r => r.UpdateStatusAsync(orderId, OrderStatus.Confirmed)).ReturnsAsync(order);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var command = new UpdateOrderStatusCommand(Guid.NewGuid(), OrderStatus.Shipped);

        _orderRepositoryMock.Setup(r => r.UpdateStatusAsync(It.IsAny<Guid>(), It.IsAny<OrderStatus>())).ReturnsAsync((Order?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectArguments()
    {
        var orderId = Guid.NewGuid();
        var command = new UpdateOrderStatusCommand(orderId, OrderStatus.Shipped);

        _orderRepositoryMock.Setup(r => r.UpdateStatusAsync(orderId, OrderStatus.Shipped)).ReturnsAsync(new Order());

        await _handler.Handle(command, CancellationToken.None);

        _orderRepositoryMock.Verify(r => r.UpdateStatusAsync(orderId, OrderStatus.Shipped), Times.Once);
    }
}

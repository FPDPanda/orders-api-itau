using Moq;
using Xunit;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Orders;
using OrdersApi.Queries.Orders.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class GetOrderByIdHandlerTests
{
    private readonly Mock<IOrderRepository> _repositoryMock = new();
    private readonly GetOrderByIdHandler _handler;

    public GetOrderByIdHandlerTests()
    {
        _handler = new GetOrderByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var expected = new Order { Id = orderId };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(orderId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync((Order?)null);

        var result = await _handler.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdToRepository()
    {
        var orderId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Order?)null);

        await _handler.Handle(new GetOrderByIdQuery(orderId), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByIdAsync(orderId), Times.Once);
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrdersApi.Controllers;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Orders;

namespace OrdersApi.Tests.Controllers;

public class OrdersControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly OrdersController _controller;

    public OrdersControllerTests()
    {
        _controller = new OrdersController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateOrder_ShouldReturnCreatedAtAction()
    {
        var order = new Order { Id = Guid.NewGuid() };
        var request = new CreateOrderRequest(OrderType.Standard, [Guid.NewGuid()], "user@test.com", "https://tracking.com");

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateOrderCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _controller.CreateOrder(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(order, created.Value);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnOk_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _controller.GetOrder(orderId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, ok.Value);
    }

    [Fact]
    public async Task GetOrder_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        var orderId = Guid.NewGuid();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetOrderByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _controller.GetOrder(orderId);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task AddItem_ShouldReturnOk_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var order = new Order { Id = orderId };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AddOrderItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _controller.AddItem(orderId, itemId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, ok.Value);
    }

    [Fact]
    public async Task AddItem_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<AddOrderItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _controller.AddItem(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task RemoveItem_ShouldReturnNoContent_WhenRemoved()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RemoveOrderItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.RemoveItem(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task RemoveItem_ShouldReturnNotFound_WhenNotRemoved()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<RemoveOrderItemCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.RemoveItem(Guid.NewGuid(), Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnOk_WhenOrderExists()
    {
        var orderId = Guid.NewGuid();
        var order = new Order { Id = orderId, Status = OrderStatus.Confirmed };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var result = await _controller.UpdateStatus(orderId, new UpdateOrderStatusRequest(OrderStatus.Confirmed));

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(order, ok.Value);
    }

    [Fact]
    public async Task UpdateStatus_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateOrderStatusCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Order?)null);

        var result = await _controller.UpdateStatus(Guid.NewGuid(), new UpdateOrderStatusRequest(OrderStatus.Shipped));

        Assert.IsType<NotFoundResult>(result);
    }
}

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrdersApi.Commands.Discounts;
using OrdersApi.Controllers;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Dtos;
using OrdersApi.Queries.Discounts;
using OrdersApi.Requests;

namespace OrdersApi.Tests.Controllers;

public class DiscountsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly DiscountsController _controller;

    public DiscountsControllerTests()
    {
        _controller = new DiscountsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetDiscounts_ShouldReturnOkWithList()
    {
        var discounts = new List<Discount>
        {
            new() { Id = Guid.NewGuid(), OrderType = OrderType.Standard, Rate = 0m, Active = true }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetDiscountsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(discounts);

        var result = await _controller.GetDiscounts();

        var ok = Assert.IsType<OkObjectResult>(result);
        var dtos = Assert.IsType<List<DiscountResponse>>(ok.Value);
        Assert.Single(dtos);
        Assert.Equal(discounts[0].Id, dtos[0].Id);
    }

    [Fact]
    public async Task CreateDiscount_ShouldReturnCreated()
    {
        var discount = new Discount
        {
            Id           = Guid.NewGuid(),
            OrderType    = OrderType.Subscription,
            DiscountType = DiscountType.Percentage,
            Rate         = -0.10m,
            Active       = true
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateDiscountCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(discount);

        var request = new CreateDiscountRequest(OrderType.Subscription, DiscountType.Percentage, -0.10m);
        var result = await _controller.CreateDiscount(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        var dto = Assert.IsType<DiscountResponse>(created.Value);
        Assert.Equal(discount.Id, dto.Id);
        Assert.Equal("Subscription", dto.OrderType);
        Assert.Equal(-0.10m, dto.Rate);
    }

    [Fact]
    public async Task DeactivateDiscount_ShouldReturnNoContent_WhenFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeactivateDiscountCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.DeactivateDiscount(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeactivateDiscount_ShouldReturnNotFound_WhenNotFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeactivateDiscountCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.DeactivateDiscount(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}

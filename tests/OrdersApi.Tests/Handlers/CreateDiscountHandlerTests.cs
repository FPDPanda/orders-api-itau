using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Commands.Discounts;
using OrdersApi.Commands.Discounts.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class CreateDiscountHandlerTests
{
    private readonly Mock<IDiscountRepository> _repositoryMock = new();
    private readonly CreateDiscountHandler _handler;

    public CreateDiscountHandlerTests()
    {
        _handler = new CreateDiscountHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCreateDiscountWithCorrectFields()
    {
        var command = new CreateDiscountCommand(OrderType.Subscription, DiscountType.Percentage, -0.10m);
        var created = new Discount
        {
            Id           = Guid.NewGuid(),
            OrderType    = OrderType.Subscription,
            DiscountType = DiscountType.Percentage,
            Rate         = -0.10m,
            Active       = true
        };

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Discount>())).ReturnsAsync(created);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(OrderType.Subscription, result.OrderType);
        Assert.Equal(-0.10m, result.Rate);
        Assert.True(result.Active);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryCreateOnce()
    {
        var command = new CreateDiscountCommand(OrderType.Express, DiscountType.Value, 5.00m);

        _repositoryMock.Setup(r => r.CreateAsync(It.IsAny<Discount>())).ReturnsAsync(new Discount());

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Discount>()), Times.Once);
    }
}

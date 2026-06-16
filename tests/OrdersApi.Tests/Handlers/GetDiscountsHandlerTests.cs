using Moq;
using Xunit;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Discounts;
using OrdersApi.Queries.Discounts.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class GetDiscountsHandlerTests
{
    private readonly Mock<IDiscountRepository> _repositoryMock = new();
    private readonly GetDiscountsHandler _handler;

    public GetDiscountsHandlerTests()
    {
        _handler = new GetDiscountsHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnActiveDiscounts()
    {
        var discounts = new List<Discount>
        {
            new() { Id = Guid.NewGuid(), OrderType = OrderType.Standard, Rate = 0m, Active = true },
            new() { Id = Guid.NewGuid(), OrderType = OrderType.Express,  Rate = 0.15m, Active = true }
        };

        _repositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(discounts);

        var result = await _handler.Handle(new GetDiscountsQuery(), CancellationToken.None);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task Handle_ShouldReturnEmptyList_WhenNoActiveDiscounts()
    {
        _repositoryMock.Setup(r => r.GetAllActiveAsync()).ReturnsAsync(new List<Discount>());

        var result = await _handler.Handle(new GetDiscountsQuery(), CancellationToken.None);

        Assert.Empty(result);
    }
}

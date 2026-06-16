using Microsoft.EntityFrameworkCore;
using Xunit;
using OrdersApi.Data;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Implementations;

namespace OrdersApi.Tests.Repository;

public class DiscountRepositoryTests
{
    private static OrdersDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    private static Discount NewDiscount(OrderType type = OrderType.Standard, decimal rate = 0m, bool active = true) => new()
    {
        OrderType    = type,
        DiscountType = DiscountType.Percentage,
        Rate         = rate,
        Active       = active
    };

    [Fact]
    public async Task CreateAsync_ShouldAssignIdAndPersist()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        var result = await repo.CreateAsync(NewDiscount());

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(1, await context.Discounts.CountAsync());
    }

    [Fact]
    public async Task GetAllActiveAsync_ShouldReturnOnlyActiveDiscounts()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        await repo.CreateAsync(NewDiscount(OrderType.Standard, active: true));
        await repo.CreateAsync(NewDiscount(OrderType.Express,  active: false));

        var result = await repo.GetAllActiveAsync();

        Assert.Single(result);
        Assert.Equal(OrderType.Standard, result[0].OrderType);
    }

    [Fact]
    public async Task GetActiveByOrderTypeAsync_ShouldReturnActiveDiscount()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        await repo.CreateAsync(NewDiscount(OrderType.Express, rate: 0.15m));

        var result = await repo.GetActiveByOrderTypeAsync(OrderType.Express);

        Assert.NotNull(result);
        Assert.Equal(0.15m, result.Rate);
    }

    [Fact]
    public async Task GetActiveByOrderTypeAsync_ShouldReturnNull_WhenNoActiveDiscount()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        await repo.CreateAsync(NewDiscount(OrderType.Express, active: false));

        var result = await repo.GetActiveByOrderTypeAsync(OrderType.Express);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeactivateAsync_ShouldSetActiveFalse()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        var created = await repo.CreateAsync(NewDiscount());

        var result = await repo.DeactivateAsync(created.Id);

        Assert.True(result);
        var updated = await context.Discounts.FindAsync(created.Id);
        Assert.False(updated!.Active);
    }

    [Fact]
    public async Task DeactivateAsync_ShouldReturnFalse_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new DiscountRepository(context);

        var result = await repo.DeactivateAsync(Guid.NewGuid());

        Assert.False(result);
    }
}

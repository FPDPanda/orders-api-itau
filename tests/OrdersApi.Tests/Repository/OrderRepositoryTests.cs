using Microsoft.EntityFrameworkCore;
using Xunit;
using OrdersApi.Data;
using OrdersApi.Domain.Enums;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Implementations;

namespace OrdersApi.Tests.Repository;

public class OrderRepositoryTests
{
    private static OrdersDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    private static Order NewOrder() => new()
    {
        Type = OrderType.Standard,
        OriginalValue = 100m,
        DebitedValue = 100m,
        User = "user@test.com",
        Status = OrderStatus.New,
        TrackingURL = "https://tracking.com"
    };

    [Fact]
    public async Task CreateAsync_ShouldAssignIdAndCreationDateTime()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var result = await repo.CreateAsync(NewOrder());

        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default, result.CreationDateTime);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistOrder()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        await repo.CreateAsync(NewOrder());

        Assert.Equal(1, await context.Orders.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder_WhenFound()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var created = await repo.CreateAsync(NewOrder());

        var result = await repo.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldIncludeProducts()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var product = new Product { Id = Guid.NewGuid(), Description = "Shirt", Price = 50m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var order = NewOrder();
        order.Products.Add(product);
        var created = await repo.CreateAsync(order);

        var result = await repo.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Single(result.Products);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddItemAsync_ShouldAddProductToOrder()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var created = await repo.CreateAsync(NewOrder());
        var product = new Product { Id = Guid.NewGuid(), Description = "Jeans", Price = 80m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var result = await repo.AddItemAsync(created.Id, product.Id);

        Assert.NotNull(result);
        Assert.Single(result.Products);
        Assert.Equal(product.Id, result.Products[0].Id);
    }

    [Fact]
    public async Task AddItemAsync_ShouldReturnNull_WhenOrderNotFound()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var result = await repo.AddItemAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task AddItemAsync_ShouldReturnNull_WhenProductNotFound()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var created = await repo.CreateAsync(NewOrder());

        var result = await repo.AddItemAsync(created.Id, Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldRemoveProductAndReturnTrue()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var product = new Product { Id = Guid.NewGuid(), Description = "Cap", Price = 25m };
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var order = NewOrder();
        order.Products.Add(product);
        var created = await repo.CreateAsync(order);

        var result = await repo.RemoveItemAsync(created.Id, product.Id);

        Assert.True(result);
        var updated = await repo.GetByIdAsync(created.Id);
        Assert.Empty(updated!.Products);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldReturnFalse_WhenOrderNotFound()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var result = await repo.RemoveItemAsync(Guid.NewGuid(), Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveItemAsync_ShouldReturnFalse_WhenProductNotInOrder()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var created = await repo.CreateAsync(NewOrder());

        var result = await repo.RemoveItemAsync(created.Id, Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldUpdateStatus_WhenOrderExists()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var created = await repo.CreateAsync(NewOrder());

        var result = await repo.UpdateStatusAsync(created.Id, OrderStatus.Confirmed);

        Assert.NotNull(result);
        Assert.Equal(OrderStatus.Confirmed, result.Status);
    }

    [Fact]
    public async Task UpdateStatusAsync_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        await using var context = CreateContext();
        var repo = new OrderRepository(context);

        var result = await repo.UpdateStatusAsync(Guid.NewGuid(), OrderStatus.Shipped);

        Assert.Null(result);
    }
}

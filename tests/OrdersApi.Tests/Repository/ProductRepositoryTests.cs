using Microsoft.EntityFrameworkCore;
using Xunit;
using OrdersApi.Data;
using OrdersApi.Domain.Models;
using OrdersApi.Repository.Implementations;

namespace OrdersApi.Tests.Repository;

public class ProductRepositoryTests
{
    private static OrdersDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<OrdersDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
            .Options);

    [Fact]
    public async Task CreateAsync_ShouldAssignNewId()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.CreateAsync(new Product { Description = "Test", Price = 10m });

        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistProduct()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        await repo.CreateAsync(new Product { Description = "Sneaker", Price = 150m, ImageURL = "https://img.com" });

        Assert.Equal(1, await context.Products.CountAsync());
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct_WhenFound()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var created = await repo.CreateAsync(new Product { Description = "Hat", Price = 20m });

        var result = await repo.GetByIdAsync(created.Id);

        Assert.NotNull(result);
        Assert.Equal(created.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateFields()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var created = await repo.CreateAsync(new Product { Description = "Old", Price = 10m, ImageURL = "old.png" });

        var result = await repo.UpdateAsync(created.Id, new Product
        {
            Description = "New",
            Price = 99m,
            ImageURL = "new.png"
        });

        Assert.NotNull(result);
        Assert.Equal("New", result.Description);
        Assert.Equal(99m, result.Price);
        Assert.Equal("new.png", result.ImageURL);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.UpdateAsync(Guid.NewGuid(), new Product { Description = "X", Price = 1m });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveProduct_AndReturnTrue()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var created = await repo.CreateAsync(new Product { Description = "Cap", Price = 30m });

        var result = await repo.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Equal(0, await context.Products.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);

        var result = await repo.DeleteAsync(Guid.NewGuid());

        Assert.False(result);
    }

    [Fact]
    public async Task CreateAsync_ShouldPreserveOrderId()
    {
        await using var context = CreateContext();
        var repo = new ProductRepository(context);
        var orderId = Guid.NewGuid();

        var result = await repo.CreateAsync(new Product { Description = "Test", Price = 10m, OrderId = orderId });

        Assert.Equal(orderId, result.OrderId);
    }
}

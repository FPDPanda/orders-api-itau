using Moq;
using Xunit;
using OrdersApi.Domain.Models;
using OrdersApi.Commands.Products;
using OrdersApi.Commands.Products.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class UpdateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _handler = new UpdateProductHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldPassUpdatedFieldsToRepository()
    {
        var productId = Guid.NewGuid();
        var command = new UpdateProductCommand(productId, "New Sneaker", "https://new.img", "New desc", 99m);

        _repositoryMock
            .Setup(r => r.UpdateAsync(productId, It.IsAny<Product>()))
            .ReturnsAsync((Guid _, Product p) => p);

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.UpdateAsync(
            productId,
            It.Is<Product>(p =>
                p.Name        == "New Sneaker"    &&
                p.ImageURL    == "https://new.img" &&
                p.Description == "New desc"        &&
                p.Price       == 99m)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnUpdatedProduct_WhenFound()
    {
        var productId = Guid.NewGuid();
        var updated   = new Product { Id = productId, Name = "Updated Cap", Description = "Updated" };
        var command   = new UpdateProductCommand(productId, "Updated Cap", "url", "Updated", 10m);

        _repositoryMock
            .Setup(r => r.UpdateAsync(productId, It.IsAny<Product>()))
            .ReturnsAsync(updated);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal("Updated Cap", result.Name);
        Assert.Equal("Updated", result.Description);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenProductNotFound()
    {
        var productId = Guid.NewGuid();
        var command   = new UpdateProductCommand(productId, "name", "url", "desc", 10m);

        _repositoryMock
            .Setup(r => r.UpdateAsync(productId, It.IsAny<Product>()))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Null(result);
    }
}

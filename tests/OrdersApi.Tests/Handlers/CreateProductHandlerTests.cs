using Moq;
using Xunit;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Products;
using OrdersApi.Queries.Products.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class CreateProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly CreateProductHandler _handler;

    public CreateProductHandlerTests()
    {
        _handler = new CreateProductHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapCommandFieldsToProduct()
    {
        var command = new CreateProductCommand("Blue T-Shirt", "https://img.com/a.png", "Cotton t-shirt, size M", 49.90m);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync((Product p) => p);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal("Blue T-Shirt", result.Name);
        Assert.Equal("https://img.com/a.png", result.ImageURL);
        Assert.Equal("Cotton t-shirt, size M", result.Description);
        Assert.Equal(49.90m, result.Price);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryCreateOnce()
    {
        var command = new CreateProductCommand("Sneaker", "url", "desc", 10m);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(new Product());

        await _handler.Handle(command, CancellationToken.None);

        _repositoryMock.Verify(r => r.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnProductFromRepository()
    {
        var expected = new Product { Id = Guid.NewGuid() };
        var command = new CreateProductCommand("Cap", "url", "desc", 10m);

        _repositoryMock
            .Setup(r => r.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(expected.Id, result.Id);
    }
}

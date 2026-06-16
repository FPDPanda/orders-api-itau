using Moq;
using Xunit;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Products;
using OrdersApi.Queries.Products.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class GetProductByIdHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly GetProductByIdHandler _handler;

    public GetProductByIdHandlerTests()
    {
        _handler = new GetProductByIdHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnProduct_WhenFound()
    {
        var productId = Guid.NewGuid();
        var expected = new Product { Id = productId };

        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync(expected);

        var result = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenNotFound()
    {
        var productId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(productId))
            .ReturnsAsync((Product?)null);

        var result = await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdToRepository()
    {
        var productId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Product?)null);

        await _handler.Handle(new GetProductByIdQuery(productId), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetByIdAsync(productId), Times.Once);
    }
}

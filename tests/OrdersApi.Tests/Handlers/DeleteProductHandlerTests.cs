using Moq;
using Xunit;
using OrdersApi.Commands.Products;
using OrdersApi.Commands.Products.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class DeleteProductHandlerTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _handler = new DeleteProductHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenDeleted()
    {
        var productId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(productId))
            .ReturnsAsync(true);

        var result = await _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenNotFound()
    {
        var productId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(productId))
            .ReturnsAsync(false);

        var result = await _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldPassCorrectIdToRepository()
    {
        var productId = Guid.NewGuid();

        _repositoryMock
            .Setup(r => r.DeleteAsync(It.IsAny<Guid>()))
            .ReturnsAsync(false);

        await _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeleteAsync(productId), Times.Once);
    }
}

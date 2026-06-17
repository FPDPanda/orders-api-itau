using Moq;
using Xunit;
using OrdersApi.Commands.Discounts;
using OrdersApi.Commands.Discounts.Handlers;
using OrdersApi.Repository.Interfaces;

namespace OrdersApi.Tests.Handlers;

public class DeactivateDiscountHandlerTests
{
    private readonly Mock<IDiscountRepository> _repositoryMock = new();
    private readonly DeactivateDiscountHandler _handler;

    public DeactivateDiscountHandlerTests()
    {
        _handler = new DeactivateDiscountHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnTrue_WhenDiscountDeactivated()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        var result = await _handler.Handle(new DeactivateDiscountCommand(id), CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Handle_ShouldReturnFalse_WhenDiscountNotFound()
    {
        _repositoryMock.Setup(r => r.DeactivateAsync(It.IsAny<Guid>())).ReturnsAsync(false);

        var result = await _handler.Handle(new DeactivateDiscountCommand(Guid.NewGuid()), CancellationToken.None);

        Assert.False(result);
    }

    [Fact]
    public async Task Handle_ShouldCallRepositoryWithCorrectId()
    {
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.DeactivateAsync(id)).ReturnsAsync(true);

        await _handler.Handle(new DeactivateDiscountCommand(id), CancellationToken.None);

        _repositoryMock.Verify(r => r.DeactivateAsync(id), Times.Once);
    }
}

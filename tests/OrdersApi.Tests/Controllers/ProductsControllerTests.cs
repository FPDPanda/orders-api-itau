using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrdersApi.Controllers;
using OrdersApi.Domain.Models;
using OrdersApi.Queries.Products;

namespace OrdersApi.Tests.Controllers;

public class ProductsControllerTests
{
    private readonly Mock<IMediator> _mediatorMock = new();
    private readonly ProductsController _controller;

    public ProductsControllerTests()
    {
        _controller = new ProductsController(_mediatorMock.Object);
    }

    [Fact]
    public async Task CreateProduct_ShouldReturnCreatedAtAction()
    {
        var product = new Product { Id = Guid.NewGuid() };
        var request = new CreateProductRequest("https://img.com/a.png", "Blue T-Shirt", 49.90m);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.CreateProduct(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        Assert.Equal(product, created.Value);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnOk_WhenFound()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.GetProduct(productId);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, ok.Value);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnNotFound_WhenNotFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _controller.GetProduct(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnOk_WhenFound()
    {
        var productId = Guid.NewGuid();
        var updated = new Product { Id = productId };
        var request = new CreateProductRequest("url", "desc", 10m);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var result = await _controller.UpdateProduct(productId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updated, ok.Value);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenNotFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _controller.UpdateProduct(Guid.NewGuid(), new CreateProductRequest("url", "desc", 10m));

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNoContent_WhenDeleted()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var result = await _controller.DeleteProduct(Guid.NewGuid());

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProduct_ShouldReturnNotFound_WhenNotFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var result = await _controller.DeleteProduct(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }
}

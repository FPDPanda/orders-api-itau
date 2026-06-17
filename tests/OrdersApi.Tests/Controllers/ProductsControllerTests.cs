using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using OrdersApi.Commands.Products;
using OrdersApi.Controllers;
using OrdersApi.Domain.Models;
using OrdersApi.Dtos;
using OrdersApi.Queries.Products;
using OrdersApi.Requests;

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
        var product = new Product { Id = Guid.NewGuid(), Name = "Blue T-Shirt", ImageURL = "https://img.com/a.png", Description = "Cotton t-shirt", Price = 49.90m };
        var request = new CreateProductRequest("Blue T-Shirt", "https://img.com/a.png", "Cotton t-shirt, size M", 49.90m);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.CreateProduct(request);

        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        var dto = Assert.IsType<ProductResponse>(created.Value);
        Assert.Equal(product.Id, dto.Id);
        Assert.Equal(product.Name, dto.Name);
    }

    [Fact]
    public async Task GetProduct_ShouldReturnOk_WhenFound()
    {
        var productId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Cap", Price = 30m };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetProductByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var result = await _controller.GetProduct(productId);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ProductResponse>(ok.Value);
        Assert.Equal(productId, dto.Id);
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
        var updated = new Product { Id = productId, Name = "Sneaker", Price = 10m };
        var request = new UpdateProductRequest("Sneaker", "url", "desc", 10m);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(updated);

        var result = await _controller.UpdateProduct(productId, request);

        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<ProductResponse>(ok.Value);
        Assert.Equal(productId, dto.Id);
    }

    [Fact]
    public async Task UpdateProduct_ShouldReturnNotFound_WhenNotFound()
    {
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Product?)null);

        var result = await _controller.UpdateProduct(Guid.NewGuid(), new UpdateProductRequest("Cap", "url", "desc", 10m));

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

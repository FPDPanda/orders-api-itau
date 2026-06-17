using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Queries.Products;

namespace OrdersApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var command = new CreateProductCommand(
            request.Name,
            request.ImageURL,
            request.Description,
            request.Price);

        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { productId = product.Id }, product);
    }

    [HttpGet("{productId:guid}")]
    public async Task<IActionResult> GetProduct(Guid productId)
    {
        var product = await _mediator.Send(new GetProductByIdQuery(productId));
        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpPut("{productId:guid}")]
    public async Task<IActionResult> UpdateProduct(Guid productId, [FromBody] CreateProductRequest request)
    {
        var product = await _mediator.Send(new UpdateProductCommand(
            productId,
            request.Name,
            request.ImageURL,
            request.Description,
            request.Price));

        if (product is null)
            return NotFound();

        return Ok(product);
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid productId)
    {
        var deleted = await _mediator.Send(new DeleteProductCommand(productId));
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}

public record CreateProductRequest(
    [property: Required]
    string Name,
    [property: Required]
    string ImageURL,
    [property: Required]
    string Description,
    [property: Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
    decimal Price);

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Domain.Enums;
using OrdersApi.Queries.Discounts;

namespace OrdersApi.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class DiscountsController : ControllerBase
{
    private readonly IMediator _mediator;

    public DiscountsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetDiscounts()
    {
        var discounts = await _mediator.Send(new GetDiscountsQuery());
        return Ok(discounts);
    }

    [HttpPost]
    public async Task<IActionResult> CreateDiscount([FromBody] CreateDiscountRequest request)
    {
        var discount = await _mediator.Send(new CreateDiscountCommand(
            request.OrderType,
            request.DiscountType,
            request.Rate));

        return CreatedAtAction(nameof(GetDiscounts), new { }, discount);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeactivateDiscount(Guid id)
    {
        var deactivated = await _mediator.Send(new DeactivateDiscountCommand(id));
        if (!deactivated)
            return NotFound();

        return NoContent();
    }
}

public record CreateDiscountRequest(OrderType OrderType, DiscountType DiscountType, decimal Rate);

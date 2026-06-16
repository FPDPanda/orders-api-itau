using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Domain.Enums;
using OrdersApi.Queries.Orders;

namespace OrdersApi.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
    {
        var command = new CreateOrderCommand(
            request.Type,
            request.ProductIds,
            request.User,
            request.TrackingURL);

        var order = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, order);
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(orderId));
        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpPut("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> AddItem(Guid orderId, Guid itemId)
    {
        var order = await _mediator.Send(new AddOrderItemCommand(orderId, itemId));
        if (order is null)
            return NotFound();

        return Ok(order);
    }

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid orderId, Guid itemId)
    {
        var removed = await _mediator.Send(new RemoveOrderItemCommand(orderId, itemId));
        if (!removed)
            return NotFound();

        return NoContent();
    }
}

public record CreateOrderRequest(
    OrderType Type,
    IReadOnlyList<Guid> ProductIds,
    string User,
    string TrackingURL);

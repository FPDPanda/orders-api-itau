using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrdersApi.Commands.Orders;
using OrdersApi.Dtos;
using OrdersApi.Queries.Orders;
using OrdersApi.Requests;

namespace OrdersApi.Controllers;

[Authorize]
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
        return CreatedAtAction(nameof(GetOrder), new { orderId = order.Id }, OrderResponse.From(order));
    }

    [HttpGet("{orderId:guid}")]
    public async Task<IActionResult> GetOrder(Guid orderId)
    {
        var order = await _mediator.Send(new GetOrderByIdQuery(orderId));
        if (order is null)
            return NotFound();

        return Ok(OrderResponse.From(order));
    }

    [HttpPut("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> AddItem(Guid orderId, Guid itemId)
    {
        try
        {
            var order = await _mediator.Send(new AddOrderItemCommand(orderId, itemId));
            if (order is null)
                return NotFound();

            return Ok(OrderResponse.From(order));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpDelete("{orderId:guid}/items/{itemId:guid}")]
    public async Task<IActionResult> RemoveItem(Guid orderId, Guid itemId)
    {
        try
        {
            var removed = await _mediator.Send(new RemoveOrderItemCommand(orderId, itemId));
            if (!removed)
                return NotFound();

            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }

    [HttpPatch("{orderId:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid orderId, [FromBody] UpdateOrderStatusRequest request)
    {
        try
        {
            var order = await _mediator.Send(new UpdateOrderStatusCommand(orderId, request.Status));
            if (order is null)
                return NotFound();

            return Ok(OrderResponse.From(order));
        }
        catch (InvalidOperationException ex)
        {
            return UnprocessableEntity(ex.Message);
        }
    }
}

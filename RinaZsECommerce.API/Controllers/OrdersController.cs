using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Orders.Commands.CancelOrder;
using RinaZsECommerce.Application.Features.Orders.Commands.Checkout;
using RinaZsECommerce.Application.Features.Orders.Queries.GetOrderById;
using RinaZsECommerce.Application.Features.Orders.Queries.GetOrderHistory;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetOrderHistory()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetOrderHistoryQuery(userId));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetOrderByIdQuery(userId, id));
        if (result == null) return NotFound(new { Message = "Pesanan tidak ditemukan." });

        return Ok(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var command = new CheckoutCommand(userId, request.Address, request.Description);
            var result = await _mediator.Send(command);
            return Ok(new { OrderId = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> CancelOrder(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var success = await _mediator.Send(new CancelOrderCommand(userId, id));
            return Ok(new { Success = success, Message = "Pesanan berhasil dibatalkan dan stok dikembalikan." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

public record CheckoutRequest(string Address, string? Description);

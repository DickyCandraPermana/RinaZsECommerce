using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Orders.Commands.Checkout;

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

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var command = new CheckoutCommand(userId, request.Address, request.Description);
        var result = await _mediator.Send(command);
        return Ok(new { OrderId = result });
    }
}

public record CheckoutRequest(string Address, string? Description);

using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Carts.Commands.AddToCart;
using RinaZsECommerce.Application.Features.Carts.Queries.GetCartItems;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CartsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCartItems()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetCartItemsQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var command = new AddToCartCommand(userId, request.ProductId, request.Quantity);
        var result = await _mediator.Send(command);
        return Ok(new { CartItemId = result });
    }
}

public record AddToCartRequest(Guid ProductId, int Quantity);

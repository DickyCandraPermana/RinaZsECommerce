using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Carts.Commands.AddToCart;
using RinaZsECommerce.Application.Features.Carts.Commands.ClearCart;
using RinaZsECommerce.Application.Features.Carts.Commands.RemoveItem;
using RinaZsECommerce.Application.Features.Carts.Commands.UpdateQuantity;
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

    [HttpPut("items/{id}")]
    public async Task<IActionResult> UpdateQuantity(Guid id, [FromBody] UpdateQuantityRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        try
        {
            var success = await _mediator.Send(new UpdateCartItemQuantityCommand(userId, id, request.Quantity));
            return Ok(new { Success = success });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpDelete("items/{id}")]
    public async Task<IActionResult> RemoveItem(Guid id)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var success = await _mediator.Send(new RemoveCartItemCommand(userId, id));
        return Ok(new { Success = success });
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var success = await _mediator.Send(new ClearCartCommand(userId));
        return Ok(new { Success = success });
    }
}

public record AddToCartRequest(Guid ProductId, int Quantity);
public record UpdateQuantityRequest(int Quantity);

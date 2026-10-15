using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Admin.Orders.Commands.UpdateStatus;
using RinaZsECommerce.Application.Features.Admin.Orders.Queries.GetAllOrders;
using RinaZsECommerce.Application.Features.Admin.Products.Commands.Restock;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMediator _mediator;

    public AdminController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetAllOrders([FromQuery] OrderFilter filter)
    {
        var result = await _mediator.Send(new GetAllOrdersAdminQuery(filter));
        return Ok(result);
    }

    [HttpPatch("orders/{id}/status")]
    public async Task<IActionResult> UpdateOrderStatus(Guid id, [FromBody] UpdateOrderStatusRequest request)
    {
        var success = await _mediator.Send(new UpdateOrderStatusCommand(id, request.Status));
        if (!success) return NotFound(new { Message = "Pesanan tidak ditemukan." });

        return Ok(new { Success = true, Message = $"Status pesanan diperbarui menjadi {request.Status}." });
    }

    [HttpPost("products/{id}/stock")]
    public async Task<IActionResult> RestockProduct(Guid id, [FromBody] RestockRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var adminUserId)) return Unauthorized();

        try
        {
            var success = await _mediator.Send(new RestockProductCommand(adminUserId, id, request.Amount, request.SupplierId));
            return Ok(new { Success = success, Message = "Stok berhasil ditambahkan ke inventaris." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}

public record UpdateOrderStatusRequest(OrderStatus Status);
public record RestockRequest(int Amount, Guid? SupplierId);

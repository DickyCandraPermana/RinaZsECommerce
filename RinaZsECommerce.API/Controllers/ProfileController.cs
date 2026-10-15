using System;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Profile.Commands.UpdateAddress;
using RinaZsECommerce.Application.Features.Profile.Commands.UpdateUserProfile;
using RinaZsECommerce.Application.Features.Profile.Queries.GetUserProfile;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProfileController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetProfile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var result = await _mediator.Send(new GetUserProfileQuery(userId));
        if (result == null) return NotFound(new { Message = "Profil tidak ditemukan." });

        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var success = await _mediator.Send(new UpdateUserProfileCommand(userId, request.FirstName, request.LastName, request.PhoneNumber));
        return Ok(new { Success = success });
    }

    [HttpPut("address")]
    public async Task<IActionResult> UpdateAddress([FromBody] UpdateAddressRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

        var success = await _mediator.Send(new UpdateAddressCommand(userId, request.Address));
        return Ok(new { Success = success });
    }
}

public record UpdateProfileRequest(string? FirstName, string? LastName, string? PhoneNumber);
public record UpdateAddressRequest(string Address);

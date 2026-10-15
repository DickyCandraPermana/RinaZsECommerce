using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RinaZsECommerce.Application.Features.Categories.Commands.CreateCategory;
using RinaZsECommerce.Application.Features.Categories.Queries.GetCategories;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetCategoriesQuery());
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request)
    {
        var result = await _mediator.Send(new CreateCategoryCommand(request.Name, request.Description));
        return Ok(new { CategoryId = result });
    }
}

public record CreateCategoryRequest(string Name, string? Description);

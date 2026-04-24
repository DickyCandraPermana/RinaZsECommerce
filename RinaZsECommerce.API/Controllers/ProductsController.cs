using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using RinaZsECommerce.Application.Features.Products.Queries.GetAllProducts;
using RinaZsECommerce.Application.Features.Products.Queries.GetProductById;
using RinaZsECommerce.Application.Features.Products.Commands.CreateProduct;
using RinaZsECommerce.Application.Features.Products.Commands.UpdateProduct;
using RinaZsECommerce.Application.Features.Products.Commands.DeleteProduct;
using RinaZsECommerce.Domain.Entities.Filter;

namespace RinaZsECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] ProductFilter filter)
    {
        var result = await _mediator.Send(new GetAllProductsQuery(filter));
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = result }, result);
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        if (id != command.Id) return BadRequest("ID mismatch");
        var result = await _mediator.Send(command);
        return Ok(result);
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));
        return Ok(result);
    }
}

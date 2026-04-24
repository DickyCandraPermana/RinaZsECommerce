using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using RinaZsECommerce.Application.DTOs;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductResponse>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var pagedProducts = await _unitOfWork.Products.GetPaginatedAsync(request.Filter);
        var products = pagedProducts.Items;
        
        // Manual mapping for MVP
        return products.Select(p => new ProductResponse(
            p.Id,
            p.Name,
            p.Price,
            p.Stock,
            p.Category?.Name ?? "Unknown",
            p.Images?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
        ));
    }
}

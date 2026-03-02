using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class ReviewRepository : GenericRepository<Review, ReviewFilter>, IReviewRepository
{
  public ReviewRepository(AppDbContext context) : base(context) { }

  protected override IQueryable<Review> ApplyFilter(IQueryable<Review> query, ReviewFilter filter)
  {
    if (!string.IsNullOrEmpty(filter.SearchTerm))
    {
      query = query.Where(r => r.Title.Contains(filter.SearchTerm));
      query = query.Where(r => r.Comment != null && r.Comment.Contains(filter.SearchTerm));
    }

    if (filter.Rating > 0)
    {
      query = query.Where(r => r.Rating > filter.Rating);
    }

    query = filter.SortBy?.ToLower() switch
    {
      "rating" => filter.IsAscending ? query.OrderBy(p => p.Rating) : query.OrderByDescending(p => p.Rating),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }
}

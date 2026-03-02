using System;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Infrastructure.Persistence.Repositories;

public class VoucherRepository : GenericRepository<Voucher, VoucherFilter>, IVoucherRepository
{
  public VoucherRepository(AppDbContext context) : base(context) { }

  protected override IQueryable<Voucher> ApplyFilter(IQueryable<Voucher> query, VoucherFilter filter)
  {
    if (filter.MinAmount > 0)
    {
      query = query.Where(v => v.Amount > filter.MinAmount);
    }
    if (filter.MinAmount > 0)
    {
      query = query.Where(v => v.Amount < filter.MaxAmount);
    }
    if (filter.MinSpend > 0)
    {
      query = query.Where(v => v.MinSpend <= filter.MinSpend);
    }
    if (filter.UserId != null)
    {
      query = query.Where(v => v.UserId == filter.UserId);
    }
    if (filter.ValidStart != null)
    {
      query = query.Where(v => v.ValidStart > filter.ValidStart);
    }
    if (filter.MaxDiscount > 0)
    {
      query = query.Where(v => v.MaxDiscount > filter.MaxDiscount);
    }
    if (filter.Expired != null)
    {
      query = query.Where(v => v.Expired < filter.Expired);
    }

    query = filter.SortBy?.ToLower() switch
    {
      "amount" => filter.IsAscending ? query.OrderBy(v => v.Amount) : query.OrderByDescending(v => v.Amount),
      "min_spend" => filter.IsAscending ? query.OrderBy(v => v.MinSpend) : query.OrderByDescending(v => v.MinSpend),
      "max_discount" => filter.IsAscending ? query.OrderBy(v => v.MaxDiscount) : query.OrderByDescending(v => v.MaxDiscount),
      _ => filter.IsAscending ? query.OrderBy(p => p.CreatedAt) : query.OrderByDescending(p => p.CreatedAt),
    };

    return query;
  }
}

using System;
using System.Diagnostics.Eventing.Reader;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IProductRepository : IGenericRepository<Product, ProductFilter>
{
  Task<IEnumerable<Product>> GetActiveProductsAsync();

  Task<bool> UpdateStockAsync(Guid productId, int quantity);
}

using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Domain.Interfaces;

public interface ICartItemRepository : IGenericRepository<CartItem, BaseFilter>
{
  Task<IEnumerable<CartItem>> GetCartItemsByUserAsync(Guid userId);
  Task<IEnumerable<CartItem>> GetActiveCartItemsByUserAsync(Guid userId);
  Task ClearCartAsync(Guid userId);
}

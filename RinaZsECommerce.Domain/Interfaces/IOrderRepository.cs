using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order, OrderFilter>
{
  Task<IEnumerable<Order>> GetOrdersWithDetailsAsync();
  Task<OrderDetail?> GetDetailByIdAsync(Guid orderDetailId);
  Task<OrderDetail?> GetDetailWithOrderAsync(Guid orderDetailId);
}

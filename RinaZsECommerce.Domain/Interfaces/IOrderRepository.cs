using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Enums;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IOrderRepository : IGenericRepository<Order, OrderFilter>
{
  Task<OrderDetail?> GetDetailByIdAsync(Guid orderDetailId);
  Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(Guid orderId);
  Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderAndProductAsync(Guid orderId, Guid productId);
}

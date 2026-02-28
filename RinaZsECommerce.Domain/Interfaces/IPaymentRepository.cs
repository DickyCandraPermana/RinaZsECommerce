using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;

namespace RinaZsECommerce.Domain.Interfaces;

public interface IPaymentRepository : IGenericRepository<Payment, PaymentFilter>
{
}

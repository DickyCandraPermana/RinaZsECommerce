using System;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Domain.Interfaces;

public interface ICategoryRepository : IGenericRepository<Category, BaseFilter>
{

}

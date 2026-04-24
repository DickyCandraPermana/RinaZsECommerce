using RinaZsECommerce.Domain.Entities;

namespace RinaZsECommerce.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}

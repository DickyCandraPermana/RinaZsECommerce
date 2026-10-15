using System;

namespace RinaZsECommerce.Application.DTOs;

public record CategoryResponse(
    Guid Id,
    string Name,
    string? Description,
    int ProductCount
);

using System;
using System.Collections.Generic;

namespace RinaZsECommerce.Application.DTOs;

public record ProductResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal Price,
    int Stock,
    decimal Rating,
    Guid CategoryId,
    string CategoryName,
    List<string> ImageUrls
);

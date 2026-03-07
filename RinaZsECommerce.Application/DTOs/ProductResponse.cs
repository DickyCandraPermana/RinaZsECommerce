using System;

namespace RinaZsECommerce.Application.DTOs;

public record ProductResponse(
    Guid Id,
    string Name,
    decimal Price,
    int Stock,
    string CategoryName, // Hasil "flattening" dari Category.Name
    List<string> ImageUrls
);
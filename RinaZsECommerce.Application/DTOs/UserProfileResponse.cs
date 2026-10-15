using System;

namespace RinaZsECommerce.Application.DTOs;

public record UserProfileResponse(
    Guid UserId,
    Guid ProfileId,
    string Username,
    string Email,
    string Role,
    string? FirstName,
    string? LastName,
    string? Address,
    string? PhoneNumber,
    bool Verified
);

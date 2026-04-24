using MediatR;
using RinaZsECommerce.Application.DTOs;

namespace RinaZsECommerce.Application.Features.Users.Commands.RegisterUser;

public record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Guid>;

using MediatR;

namespace RinaZsECommerce.Application.Features.Users.Commands.LoginUser;

public record LoginUserCommand(
    string Email,
    string Password
) : IRequest<string>; // Returns JWT Token

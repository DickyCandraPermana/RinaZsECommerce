using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RinaZsECommerce.Application.Interfaces;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Exceptions;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Users.Commands.LoginUser;

public class LoginUserHandler : IRequestHandler<LoginUserCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IPasswordHasher<User> _passwordHasher;

    public LoginUserHandler(IUnitOfWork unitOfWork, IJwtTokenGenerator jwtTokenGenerator)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenGenerator = jwtTokenGenerator;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<string> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var pagedUsers = await _unitOfWork.Users.GetPaginatedAsync(new UserFilter { Email = request.Email });
        var user = pagedUsers.Items.FirstOrDefault();

        if (user == null)
            throw new InvalidCredentialsException();

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        return _jwtTokenGenerator.GenerateToken(user);
    }
}

using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Interfaces;

namespace RinaZsECommerce.Application.Features.Users.Commands.RegisterUser;

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<Guid> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var isUnique = await _unitOfWork.Users.IsEmailUniqueAsync(request.Email);
        if (!isUnique)
            throw new Exception("Email already exists"); // TODO: Use custom exception

        var profileId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            Role = "Customer",
            ProfileId = profileId,
            PasswordHash = ""
        };
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        var profile = new UserProfile
        {
            Id = profileId,
            UserId = user.Id,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Verified = false
        };

        user.UserProfile = profile;
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        return user.Id;
    }
}

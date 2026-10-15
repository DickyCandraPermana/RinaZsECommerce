using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using RinaZsECommerce.Application.Features.Users.Commands.LoginUser;
using RinaZsECommerce.Application.Interfaces;
using RinaZsECommerce.Domain.Common;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Entities.Filter;
using RinaZsECommerce.Domain.Exceptions;
using RinaZsECommerce.Domain.Interfaces;
using Xunit;

namespace RinaZsECommerce.UnitTests;

public class LoginUserHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IJwtTokenGenerator> _jwtMock;
    private readonly LoginUserHandler _handler;

    public LoginUserHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _jwtMock = new Mock<IJwtTokenGenerator>();
        _handler = new LoginUserHandler(_unitOfWorkMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_UserNotFound_ThrowsInvalidCredentialsException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.GetPaginatedAsync(It.IsAny<UserFilter>()))
            .ReturnsAsync(PagedResult<User>.Create(new List<User>(), 0, 1, 10));

        var command = new LoginUserCommand("nonexistent@example.com", "Password123!");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<InvalidCredentialsException>();
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsJwtToken()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "user@example.com",
            Username = "validuser",
            Role = "Customer",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "Password123!")
        };

        _userRepoMock
            .Setup(r => r.GetPaginatedAsync(It.IsAny<UserFilter>()))
            .ReturnsAsync(PagedResult<User>.Create(new List<User> { user }, 1, 1, 10));

        _jwtMock
            .Setup(j => j.GenerateToken(user))
            .Returns("mocked.jwt.token");

        var command = new LoginUserCommand("user@example.com", "Password123!");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().Be("mocked.jwt.token");
    }
}

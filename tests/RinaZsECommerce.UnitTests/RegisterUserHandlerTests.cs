using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using RinaZsECommerce.Application.Features.Users.Commands.RegisterUser;
using RinaZsECommerce.Domain.Entities;
using RinaZsECommerce.Domain.Exceptions;
using RinaZsECommerce.Domain.Interfaces;
using Xunit;

namespace RinaZsECommerce.UnitTests;

public class RegisterUserHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly RegisterUserHandler _handler;

    public RegisterUserHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _unitOfWorkMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _handler = new RegisterUserHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task Handle_EmailAlreadyExists_ThrowsEmailAlreadyExistsException()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.IsEmailUniqueAsync("existing@example.com"))
            .ReturnsAsync(false);

        var command = new RegisterUserCommand("user1", "existing@example.com", "Password123!", "John", "Doe");

        // Act
        var act = async () => await _handler.Handle(command, CancellationToken.None);

        // Assert
        var exceptionAssertion = await act.Should().ThrowAsync<EmailAlreadyExistsException>();
        exceptionAssertion.Which.Email.Should().Be("existing@example.com");
    }

    [Fact]
    public async Task Handle_ValidRegistration_CreatesUserAndReturnsId()
    {
        // Arrange
        _userRepoMock
            .Setup(r => r.IsEmailUniqueAsync("new@example.com"))
            .ReturnsAsync(true);

        _unitOfWorkMock
            .Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        var command = new RegisterUserCommand("newuser", "new@example.com", "SecurePass123!", "Jane", "Doe");

        // Act
        var userId = await _handler.Handle(command, CancellationToken.None);

        // Assert
        userId.Should().NotBeEmpty();
        _userRepoMock.Verify(r => r.AddAsync(It.Is<User>(u => u.Email == "new@example.com")), Times.Once);
        _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
    }
}

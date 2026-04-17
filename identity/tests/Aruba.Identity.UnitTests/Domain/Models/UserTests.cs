using Aruba.Identity.Domain.Exceptions;
using Aruba.Identity.Domain.Models;
using FluentAssertions;

namespace Aruba.Identity.UnitTests.Domain.Models;

public class UserTests
{
    [Fact]
    public void User_Constructor_ShouldCreateUser_WithValidData()
    {
        // Given-When
        var user = new User
        (
            "mario.rossi",
            "mario.rossi@gmail.com",
            "hashedpwd123",
            "Via Roma 10"
        );

        // Then
        user.Id.Should().NotBeNullOrWhiteSpace();
        user.UserName.Should().Be("mario.rossi");
        user.Email.Should().Be("mario.rossi@gmail.com");
        user.PasswordHash.Should().Be("hashedpwd123");
        user.Address.Should().Be("Via Roma 10");
    }

    [Fact]
    public void User_Constructor_ShouldThrow_WhenUserNameIsNull()
    {
        // Given-When
        Action act = () => new User
        (
            null,
            "test@gmail.com",
            "pwd"
        );

        // Then
        act.Should().Throw<DomainException>().WithMessage("Username cannot be empty.");
    }

    [Fact]
    public void User_Constructor_ShouldThrow_WhenEmailIsNull()
    {
        // Given-When
        Action act = () => new User
        (
            "test",
            null,
            "pwd"
        );

        // Then
        act.Should().Throw<DomainException>().WithMessage("Email cannot be empty.");
    }

    [Fact]
    public void User_Constructor_ShouldThrow_WhenPasswordHashIsNull()
    {
        // Given-When
        Action act = () => new User
        (
            "test",
            "test@gmail.com",
            null
        );

        // Then
        act.Should().Throw<DomainException>().WithMessage("PasswordHash cannot be empty.");
    }

    [Fact]
    public void User_UpdateEmail_ShouldUpdateEmail_WhenValid()
    {
        // Given
        var user = new User("test", "old@gmail.com", "pwd");

        // When
        user.UpdateEmail("new@gmail.com");

        // Then
        user.Email.Should().Be("new@gmail.com");
    }

    [Fact]
    public void User_UpdateEmail_ShouldThrow_WhenEmailIsNull()
    {
        // Given
        var user = new User("test", "old@gmail.com", "pwd");

        // When
        Action act = () => user.UpdateEmail(null);

        // Then
        act.Should().Throw<DomainException>().WithMessage("Email cannot be empty.");
    }

    [Fact]
    public void User_UpdateAddress_ShouldUpdateAddress()
    {
        // Given
        var user = new User("test", "test@gmail.com", "pwd");

        // When
        user.UpdateAddress("Via Milano 20");

        // Then
        user.Address.Should().Be("Via Milano 20");
    }

    [Fact]
    public void User_ChangePassword_ShouldUpdatePassword_WhenValid()
    {
        // Given
        var user = new User("test", "test@gmail.com", "oldpwd");

        // When
        user.ChangePassword("newpwd");

        // Then
        user.PasswordHash.Should().Be("newpwd");
    }

    [Fact]
    public void User_ChangePassword_ShouldThrow_WhenPasswordIsNull()
    {
        // Given
        var user = new User("test", "test@gmail.com", "pwd");

        // When
        Action act = () => user.ChangePassword(null);

        // Then
        act.Should().Throw<DomainException>().WithMessage("PasswordHash cannot be empty.");
    }
}
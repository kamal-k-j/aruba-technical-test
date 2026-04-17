using Aruba.Identity.Api.Auth.Requests;
using Aruba.Identity.Api.Auth.Responses;
using Aruba.Identity.Api.Users.Responses;
using Aruba.Identity.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace Aruba.Identity.FunctionalTests.Auth;

public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IMongoDatabase _database;

    public AuthControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    }

    [Fact]
    public async Task AuthController_Register_ShouldCreateUser_WhenValid()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "mario.rossi",
            Email = "mario.rossi@gmail.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result.Email.Should().Be("mario.rossi@gmail.com");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenUserNameIsNull()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = null,
            Email = "valid@example.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenUserNameIsEmpty()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "",
            Email = "valid@example.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenUserNameTooLong()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = new string('a', 21),
            Email = "valid@example.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenEmailIsNull()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = null,
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenEmailIsEmpty()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = "",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenEmailInvalid()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = "invalid-email",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenPasswordIsNull()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = "valid@example.com",
            Password = null,
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenPasswordIsEmpty()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = "valid@example.com",
            Password = "",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Register_ShouldReturnBadRequest_WhenAddressTooLong()
    {
        // Given
        var payload = new RegisterRequest
        {
            UserName = "valid",
            Email = "valid@example.com",
            Password = "pwd123",
            Address = new string('a', 51)
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/register", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task AuthController_Login_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Given
        var user = new User
        (
            "mario.rossi",
            "mario.rossi@gmail.com",
            BCrypt.Net.BCrypt.HashPassword("pwd123"),
            "Via Roma 10"
        );
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new LoginRequest
        {
            Email = "mario.rossi@gmail.com",
            Password = "pwd123"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/login", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrWhiteSpace();
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task AuthController_Login_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Given
        var payload = new LoginRequest
        {
            Email = "notfound@example.com",
            Password = "pwd123"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/login", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task AuthController_Login_ShouldReturnBadRequest_WhenPasswordIsWrong()
    {
        // Given
        var user = new User
        (
            "mario.rossi",
            "mario.rossi@gmail.com",
            BCrypt.Net.BCrypt.HashPassword("pwd123")
        );
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new LoginRequest
        {
            Email = "mario.rossi@gmail.com",
            Password = "wrongpassword"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/login", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task AuthController_Login_ShouldReturnBadRequest_WhenEmailInvalid()
    {
        // Given
        var payload = new LoginRequest
        {
            Email = "invalid-email",
            Password = "pwd123"
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/login", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task AuthController_Login_ShouldReturnBadRequest_WhenPasswordIsNullOrEmpty(string password)
    {
        // Given
        var user = new User
        (
            "mario.rossi",
            "mario.rossi@gmail.com",
            BCrypt.Net.BCrypt.HashPassword("pwd123")
        );
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new LoginRequest
        {
            Email = user.Email,
            Password = password
        };

        // When
        var response = await _client.PostAsJsonAsync("/auth/login", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }
}
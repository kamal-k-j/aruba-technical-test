using Aruba.Identity.Api.Users.Requests;
using Aruba.Identity.Api.Users.Responses;
using Aruba.Identity.Domain.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Net;
using System.Net.Http.Json;

namespace Aruba.Identity.FunctionalTests.Users;

public class UsersControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IMongoDatabase _database;

    public UsersControllerTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        var scope = factory.Services.CreateScope();
        _database = scope.ServiceProvider.GetRequiredService<IMongoDatabase>();
    }

    [Fact]
    public async Task UsersController_GetById_ShouldReturnUser_WhenExists()
    {
        // Given
        var user = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync($"/users/{user.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be("mario.rossi@gmail.com");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_GetById_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.GetAsync($"/users/{nonExistingId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UsersController_GetByEmail_ShouldReturnUser_WhenExists()
    {
        // Given
        var user = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync($"/users?email={user.Email}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Email.Should().Be("mario.rossi@gmail.com");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_GetByEmail_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Given
        var email = "notfound@example.com";

        // When
        var response = await _client.GetAsync($"/users?email={email}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UsersController_GetByEmail_ShouldReturnNotFound_WhenEmailIsEmpty()
    {
        // Given-When
        var response = await _client.GetAsync("/users?email=");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UsersController_GetAll_ShouldReturnUsers_WhenExists()
    {
        // Given
        var user1 = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        var user2 = new User("marco.bianchi", "marco.bianchi@gmail.com", "pwd456");
        await TestDataSeeder.InsertUserAsync(_database, user1);
        await TestDataSeeder.InsertUserAsync(_database, user2);

        // When
        var response = await _client.GetAsync("/users/all");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().HaveCount(2);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Update_ShouldUpdateUser_WhenExists()
    {
        // Given
        var user = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = "mario.rossi",
            Email = "new.email@gmail.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<UserResponse>();
        result.Should().NotBeNull();
        result.Email.Should().Be("new.email@gmail.com");
        result.Address.Should().Be("Via Roma 10");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Update_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Given
        var payload = new UpdateUserRequest
        {
            UserName = "test",
            Email = "test@example.com",
            Password = "pwd",
            Address = "Somewhere"
        };
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.PutAsJsonAsync($"/users/{nonExistingId}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenUserNameIsNullOrEmpty(string userName)
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = userName,
            Email = "valid@example.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenUserNameTooLong()
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = new string('a', 21),
            Email = "valid@example.com",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenEmailIsNullOrEmpty(string email)
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = "valid",
            Email = email,
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenEmailInvalid()
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = "valid",
            Email = "invalid-email",
            Password = "pwd123",
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenPasswordIsNullOrEmpty(string password)
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = "valid",
            Email = "valid@example.com",
            Password = password,
            Address = "Via Roma 10"
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Update_ShouldReturnBadRequest_WhenAddressTooLong()
    {
        // Given
        var user = new User("valid", "valid@example.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);
        var payload = new UpdateUserRequest
        {
            UserName = "valid",
            Email = "valid@example.com",
            Password = "pwd123",
            Address = new string('a', 51)
        };

        // When
        var response = await _client.PutAsJsonAsync($"/users/{user.Id}", payload);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Delete_ShouldDeleteUser_WhenExists()
    {
        // Given
        var user = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.DeleteAsync($"/users/{user.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task UsersController_Delete_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Given
        var nonExistingId = ObjectId.GenerateNewId().ToString();

        // When
        var response = await _client.DeleteAsync($"/users/{nonExistingId}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnUsers_WhenEmailMatches()
    {
        // Given
        var user = new User("mario.rossi", "mario.rossi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync("/users/search?field=email&value=mario.rossi@gmail.com");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().HaveCount(1);
        result[0].Email.Should().Be("mario.rossi@gmail.com");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnUsers_WhenUserNameMatches()
    {
        // Given
        var user = new User("marco.bianchi", "marco.bianchi@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync("/users/search?field=username&value=marco.bianchi");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().HaveCount(1);
        result[0].UserName.Should().Be("marco.bianchi");
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnEmptyList_WhenNoMatch()
    {
        // Given
        var user = new User("test.user", "test.user@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync("/users/search?field=email&value=notfound@example.com");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().BeEmpty();
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnEmptyList_WhenFieldIsUnknown()
    {
        // Given
        var user = new User("test.user", "test.user@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync("/users/search?field=unknown&value=test");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().BeEmpty();
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnUsers_WhenSearchingById()
    {
        // Given
        var user = new User("test.user", "test.user@gmail.com", "pwd123");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync($"/users/search?field=id&value={user.Id}");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(user.Id);
        await TestDataSeeder.ResetUsersAsync(_database);
    }

    [Fact]
    public async Task UsersController_Search_ShouldReturnUsers_WhenSearchingByAddress()
    {
        // Given
        var user = new User("test.user", "test.user@gmail.com", "pwd123", "Via Roma 10");
        await TestDataSeeder.InsertUserAsync(_database, user);

        // When
        var response = await _client.GetAsync("/users/search?field=address&value=Via Roma 10");

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<List<UserResponse>>();
        result.Should().HaveCount(1);
        result[0].Address.Should().Be("Via Roma 10");
        await TestDataSeeder.ResetUsersAsync(_database);
    }
}
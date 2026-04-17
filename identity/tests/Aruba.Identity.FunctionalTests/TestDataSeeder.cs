using Aruba.Identity.Domain.Models;
using MongoDB.Driver;

namespace Aruba.Identity.FunctionalTests;

public static class TestDataSeeder
{
    private const string CollectionName = nameof(User);

    public static async Task ResetUsersAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<User>(CollectionName);
        await collection.DeleteManyAsync(FilterDefinition<User>.Empty);
    }

    public static async Task InsertUserAsync(IMongoDatabase database, User user)
    {
        var collection = database.GetCollection<User>(CollectionName);
        await collection.InsertOneAsync(user);
    }
}

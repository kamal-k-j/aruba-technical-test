using MongoDB.Driver;

namespace Aruba.Document.FunctionalTests;

public static class TestDataSeeder
{
    private const string CollectionName = nameof(Domain.Models.Document);

    public static async Task ResetDocumentsAsync(IMongoDatabase database)
    {
        var collection = database.GetCollection<Domain.Models.Document>(CollectionName);
        await collection.DeleteManyAsync(FilterDefinition<Domain.Models.Document>.Empty);
    }

    public static async Task InsertDocumentAsync(IMongoDatabase database, Domain.Models.Document document)
    {
        var collection = database.GetCollection<Domain.Models.Document>(CollectionName);
        await collection.InsertOneAsync(document);
    }
}

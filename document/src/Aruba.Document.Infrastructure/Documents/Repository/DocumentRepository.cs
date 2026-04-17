using MongoDB.Driver;
using System.Linq.Expressions;

namespace Aruba.Document.Infrastructure.Documents.Repository;

public class DocumentRepository : IDocumentRepository
{
    private readonly IMongoCollection<Domain.Models.Document> _collection;

    public DocumentRepository(IMongoDatabase database) => 
        _collection = database.GetCollection<Domain.Models.Document>(nameof(Domain.Models.Document));

    public Task<Domain.Models.Document> GetByIdAsync(string id) =>
        _collection.Find(x => x.Id == id).FirstOrDefaultAsync();

    public Task<List<Domain.Models.Document>> GetAllAsync() =>
        _collection.Find(_ => true).ToListAsync();

    public Task InsertAsync(Domain.Models.Document entity) =>
        _collection.InsertOneAsync(entity);

    public Task UpdateAsync(Domain.Models.Document entity) =>
        _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity);

    public Task<List<Domain.Models.Document>> FindAsync(Expression<Func<Domain.Models.Document, bool>> predicate) =>
        _collection.Find(predicate).ToListAsync();
}
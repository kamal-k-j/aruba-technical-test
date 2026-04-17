using System.Linq.Expressions;

namespace Aruba.Document.Infrastructure.Documents.Repository;

public interface IDocumentRepository
{
    Task<Domain.Models.Document> GetByIdAsync(string id);

    Task<List<Domain.Models.Document>> GetAllAsync();

    Task InsertAsync(Domain.Models.Document entity);

    Task UpdateAsync(Domain.Models.Document entity);

    Task<List<Domain.Models.Document>> FindAsync(Expression<Func<Domain.Models.Document, bool>> predicate);
}
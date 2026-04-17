using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Queries.Search;

public record SearchDocumentsQuery : IRequest<List<DocumentResult>>
{
    public required string Field { get; init; }

    public required string Value { get; init; }
}
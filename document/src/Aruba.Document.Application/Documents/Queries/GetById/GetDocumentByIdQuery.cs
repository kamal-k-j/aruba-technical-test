using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Queries.GetById;

public record GetDocumentByIdQuery : IRequest<DocumentResult>
{
    public required string Id { get; init; }
}
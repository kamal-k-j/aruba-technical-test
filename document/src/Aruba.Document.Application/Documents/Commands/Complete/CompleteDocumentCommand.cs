using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Complete;

public record CompleteDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }
}
using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Link;

public record LinkDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }

    public required string TargetId { get; init; }
}
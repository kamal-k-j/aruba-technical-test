using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Reject;

public record RejectDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }
}
using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Approve;

public record ApproveDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }
}
using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Send;

public record SendDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }
}
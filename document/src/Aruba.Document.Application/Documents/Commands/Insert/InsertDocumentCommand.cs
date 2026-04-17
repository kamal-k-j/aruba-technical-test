using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Domain.Enums;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Insert;

public record InsertDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }

    public required DocumentType Type { get; init; }

    public required string CustomerName { get; init; }

    public required string Description { get; init; }
}
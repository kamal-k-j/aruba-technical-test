using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Domain.Enums;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Generate;

public record GenerateDocumentCommand : IRequest<DocumentResult>
{
    public required string Id { get; init; }

    public required DocumentType Type { get; init; }
}
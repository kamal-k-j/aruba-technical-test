using Aruba.Document.Domain.Enums;

namespace Aruba.Document.Application.Documents.Responses;

public record DocumentResult
{
    public required string Id { get; init; }

    public required DocumentType Type { get; init; }

    public required DocumentStatus Status { get; init; }

    public required string CustomerName { get; init; }

    public required string Description { get; init; }

    public required IReadOnlyList<string> LinkedDocuments { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }
}
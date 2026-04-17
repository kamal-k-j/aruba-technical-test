using System.ComponentModel.DataAnnotations;

namespace Aruba.Document.Api.Documents.Requests;

public record UpdateDocumentRequest
{
    [Required]
    public string Type { get; init; }

    [Required]
    public string CustomerName { get; init; }

    [Required]
    public string Description { get; init; }
}
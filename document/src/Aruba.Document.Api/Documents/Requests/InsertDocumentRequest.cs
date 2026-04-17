using Aruba.Document.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Aruba.Document.Api.Documents.Requests;

public record InsertDocumentRequest
{
    [Required]
    public DocumentType Type { get; init; }

    [Required]
    public string CustomerName { get; init; }

    [Required]
    public string Description { get; init; }
}
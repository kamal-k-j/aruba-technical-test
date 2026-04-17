using Aruba.Document.Application.Documents.Responses;
using MediatR;

namespace Aruba.Document.Application.Documents.Queries.GetAll;

public record GetAllDocumentsQuery : IRequest<List<DocumentResult>>;
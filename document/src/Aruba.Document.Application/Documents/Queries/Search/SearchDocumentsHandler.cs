using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Domain.Enums;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;
using System.Linq.Expressions;

namespace Aruba.Document.Application.Documents.Queries.Search;

public class SearchDocumentsHandler : IRequestHandler<SearchDocumentsQuery, List<DocumentResult>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public SearchDocumentsHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<List<DocumentResult>> Handle(SearchDocumentsQuery request, CancellationToken cancellationToken)
    {
        var predicate = BuildPredicate(request.Field, request.Value);

        var documents = await _documentRepository.FindAsync(predicate);

        return _mapper.Map<List<DocumentResult>>(documents);
    }

    private static Expression<Func<Domain.Models.Document, bool>> BuildPredicate(string field, string value) => field.ToLower() switch
    {
        "customername" => _ => _.CustomerName == value,
        "description" => _ => _.Description == value,
        "status" => Enum.TryParse(value, true, out DocumentStatus status)
            ? _ => _.Status == status
            : _ => false,
        "type" => Enum.TryParse(value, true, out DocumentType type) 
            ? _ => _.Type == type 
            : _ => false,
        "id" => _ => _.Id == value,
        _ => _ => false
    };
}
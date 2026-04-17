using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Queries.GetAll;

public class GetAllDocumentsHandler : IRequestHandler<GetAllDocumentsQuery, List<DocumentResult>>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GetAllDocumentsHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<List<DocumentResult>> Handle(GetAllDocumentsQuery request, CancellationToken cancellationToken)
    {
        var documents = await _documentRepository.GetAllAsync();

        return _mapper.Map<List<DocumentResult>>(documents);
    }
}
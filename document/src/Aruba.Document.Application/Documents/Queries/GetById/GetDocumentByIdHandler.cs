using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Queries.GetById;

public class GetDocumentByIdHandler : IRequestHandler<GetDocumentByIdQuery, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GetDocumentByIdHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(GetDocumentByIdQuery request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        return _mapper.Map<DocumentResult>(document);
    }
}
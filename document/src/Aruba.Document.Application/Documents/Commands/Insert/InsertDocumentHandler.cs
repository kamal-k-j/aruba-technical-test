using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Insert;

public class InsertDocumentHandler : IRequestHandler<InsertDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public InsertDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(InsertDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = new Domain.Models.Document
        (
            request.Type,
            request.CustomerName,
            request.Description
        );

        await _documentRepository.InsertAsync(document);

        document = await _documentRepository.GetByIdAsync(document.Id);

        return _mapper.Map<DocumentResult>(document);
    }
}
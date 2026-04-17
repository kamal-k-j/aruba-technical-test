using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Link;

public class LinkDocumentHandler : IRequestHandler<LinkDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public LinkDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(LinkDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);

        if (document == null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        var targetDocument = await _documentRepository.GetByIdAsync(request.TargetId);

        if (targetDocument == null)
        {
            throw new NotFoundException($"Target document not found for the id specified: {request.TargetId}.");
        }

        document.LinkTo(targetDocument.Id);

        await _documentRepository.UpdateAsync(document);

        return _mapper.Map<DocumentResult>(document);
    }
}
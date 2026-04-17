using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Reject;

public class RejectDocumentHandler : IRequestHandler<RejectDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public RejectDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(RejectDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);

        if (document == null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        document.Reject();

        await _documentRepository.UpdateAsync(document);

        return _mapper.Map<DocumentResult>(document);
    }
}
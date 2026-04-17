using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Approve;

public class ApproveDocumentHandler : IRequestHandler<ApproveDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public ApproveDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(ApproveDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);

        if (document == null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        document.Approve();

        await _documentRepository.UpdateAsync(document);

        return _mapper.Map<DocumentResult>(document);
    }
}
using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Update;

public class UpdateDocumentHandler : IRequestHandler<UpdateDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public UpdateDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var document = await _documentRepository.GetByIdAsync(request.Id);

        if (document is null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        document.UpdateCustomerName(request.CustomerName);

        document.UpdateDescription(request.Description);

        document.ChangeType(request.Type);

        await _documentRepository.UpdateAsync(document);

        document = await _documentRepository.GetByIdAsync(document.Id);

        return _mapper.Map<DocumentResult>(document);
    }
}
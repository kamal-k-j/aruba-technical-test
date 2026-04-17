using Aruba.Document.Application.Documents.Responses;
using Aruba.Document.Application.Exceptions;
using Aruba.Document.Infrastructure.Documents.Repository;
using AutoMapper;
using MediatR;

namespace Aruba.Document.Application.Documents.Commands.Generate;

public class GenerateDocumentHandler : IRequestHandler<GenerateDocumentCommand, DocumentResult>
{
    private readonly IDocumentRepository _documentRepository;
    private readonly IMapper _mapper;

    public GenerateDocumentHandler(IDocumentRepository documentRepository, IMapper mapper)
    {
        _documentRepository = documentRepository;
        _mapper = mapper;
    }

    public async Task<DocumentResult> Handle(GenerateDocumentCommand request, CancellationToken cancellationToken)
    {
        var sourceDocument = await _documentRepository.GetByIdAsync(request.Id);

        if (sourceDocument is null)
        {
            throw new NotFoundException($"Document not found for the id specified: {request.Id}.");
        }

        var generatedDocument = new Domain.Models.Document
        (
            request.Type,
            sourceDocument.CustomerName,
            sourceDocument.Description
        );

        generatedDocument.LinkTo(sourceDocument.Id);

        await _documentRepository.InsertAsync(generatedDocument);

        generatedDocument = await _documentRepository.GetByIdAsync(generatedDocument.Id);

        return _mapper.Map<DocumentResult>(generatedDocument);
    }
}
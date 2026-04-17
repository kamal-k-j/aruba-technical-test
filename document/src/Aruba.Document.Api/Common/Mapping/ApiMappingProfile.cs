using Aruba.Document.Api.Documents.Requests;
using Aruba.Document.Api.Documents.Responses;
using Aruba.Document.Application.Documents.Commands.Insert;
using Aruba.Document.Application.Documents.Commands.Update;
using Aruba.Document.Application.Documents.Responses;
using AutoMapper;

namespace Aruba.Document.Api.Common.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<InsertDocumentRequest, InsertDocumentCommand>();
        CreateMap<UpdateDocumentRequest, UpdateDocumentCommand>();
        CreateMap<DocumentResult, DocumentResponse>();
    }
}
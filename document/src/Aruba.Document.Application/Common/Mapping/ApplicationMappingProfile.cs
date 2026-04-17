using Aruba.Document.Application.Documents.Responses;
using AutoMapper;

namespace Aruba.Document.Application.Common.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<Domain.Models.Document, DocumentResult>();
    }
}
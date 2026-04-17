using Aruba.Identity.Application.Users.Responses;
using Aruba.Identity.Domain.Models;
using AutoMapper;

namespace Aruba.Identity.Application.Common.Mapping;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<User, UserResult>();
    }
}
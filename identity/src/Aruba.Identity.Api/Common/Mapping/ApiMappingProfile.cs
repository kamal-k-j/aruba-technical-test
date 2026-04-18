using Aruba.Identity.Api.Auth.Requests;
using Aruba.Identity.Api.Auth.Responses;
using Aruba.Identity.Api.Users.Requests;
using Aruba.Identity.Api.Users.Responses;
using Aruba.Identity.Application.Auth.Commands.Login;
using Aruba.Identity.Application.Auth.Commands.Register;
using Aruba.Identity.Application.Auth.Responses;
using Aruba.Identity.Application.Users.Commands.Update;
using Aruba.Identity.Application.Users.Responses;
using AutoMapper;

namespace Aruba.Identity.Api.Common.Mapping;

public class ApiMappingProfile : Profile
{
    public ApiMappingProfile()
    {
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UserResult, UserResponse>();

        CreateMap<RegisterRequest, RegisterCommand>();
        CreateMap<LoginRequest, LoginCommand>();
        CreateMap<LoginResult, LoginResponse>();
    }
}
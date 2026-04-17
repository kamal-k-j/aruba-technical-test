using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetById;

public record GetUserByIdQuery : IRequest<UserResult>
{
    public required string Id { get; init; }
}
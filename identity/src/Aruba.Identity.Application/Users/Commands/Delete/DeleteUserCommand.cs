using MediatR;

namespace Aruba.Identity.Application.Users.Commands.Delete;

public record DeleteUserCommand : IRequest
{
    public required string Id { get; init; }
}
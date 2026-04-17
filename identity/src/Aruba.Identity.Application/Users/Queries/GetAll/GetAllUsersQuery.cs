using Aruba.Identity.Application.Users.Responses;
using MediatR;

namespace Aruba.Identity.Application.Users.Queries.GetAll;

public record GetAllUsersQuery : IRequest<List<UserResult>>;
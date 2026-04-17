using Aruba.Identity.Application.Exceptions;
using Aruba.Identity.Infrastructure.Users.Repository;
using MediatR;

namespace Aruba.Identity.Application.Users.Commands.Delete;

public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
{
    private readonly IUserRepository _userRepository;

    public DeleteUserHandler(IUserRepository userRepository) => _userRepository = userRepository;

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id);

        if (user is null)
        {
            throw new NotFoundException($"User not found for the id specified: {request.Id}.");
        }

        await _userRepository.DeleteAsync(request.Id);
    }
}
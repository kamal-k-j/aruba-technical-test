using Aruba.Identity.Application.Auth.Responses;
using Aruba.Identity.Application.Exceptions;
using Aruba.Identity.Infrastructure.Auth;
using Aruba.Identity.Infrastructure.Users.Repository;
using FluentValidation;
using MediatR;

namespace Aruba.Identity.Application.Auth.Commands.Login;

public class LoginHandler : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenGenerator _jwt;

    public LoginHandler(IUserRepository userRepository, IJwtTokenGenerator jwt)
    {
        _userRepository = userRepository;
        _jwt = jwt;
    }

    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user is null)
        {
            throw new NotFoundException($"User not found for the email specified: {request.Email}.");
        }

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw new ValidationException("Invalid credentials",
            [
                new()
                {
                    PropertyName= "Password",
                    ErrorMessage = "Password not matching."
                }
            ]);
        }

        var (Token, ExpiresAt) = _jwt.GenerateToken(user);

        return new LoginResult
        {
            Token = Token,
            ExpiresAt = ExpiresAt
        };
    }
}

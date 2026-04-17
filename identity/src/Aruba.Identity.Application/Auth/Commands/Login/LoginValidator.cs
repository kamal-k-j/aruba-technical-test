using FluentValidation;

namespace Aruba.Identity.Application.Auth.Commands.Login;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();
    }
}
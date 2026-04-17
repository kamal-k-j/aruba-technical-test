using FluentValidation;

namespace Aruba.Identity.Application.Auth.Commands.Register;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .NotNull()
            .MaximumLength(20);

        RuleFor(x => x.Email)
            .NotEmpty()
            .NotNull()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Address)
            .MaximumLength(50);
    }
}
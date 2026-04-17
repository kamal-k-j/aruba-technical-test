using FluentValidation;

namespace Aruba.Identity.Application.Users.Commands.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
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
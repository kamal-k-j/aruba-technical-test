using FluentValidation;

namespace Aruba.Document.Application.Documents.Commands.Update;

public class UpdateDocumentValidator : AbstractValidator<UpdateDocumentCommand>
{
    public UpdateDocumentValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull();
    }
}
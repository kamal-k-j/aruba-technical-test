using FluentValidation;

namespace Aruba.Document.Application.Documents.Commands.Insert;

public class InsertDocumentValidator : AbstractValidator<InsertDocumentCommand>
{
    public InsertDocumentValidator()
    {
        RuleFor(x => x.CustomerName)
            .NotEmpty()
            .NotNull();

        RuleFor(x => x.Description)
            .NotEmpty()
            .NotNull();
    }
}
using Aruba.Document.Domain.Enums;
using Aruba.Document.Infrastructure.Documents.Repository;
using FluentValidation;

namespace Aruba.Document.Application.Documents.Commands.Generate;

public class GenerateDocumentValidator : AbstractValidator<GenerateDocumentCommand>
{
    public GenerateDocumentValidator(IDocumentRepository repository)
    {
        RuleFor(x => x)
            .CustomAsync(async (command, context, cancellation) =>
            {
                var source = await repository.GetByIdAsync(command.Id);

                if (source is null)
                {
                    return;
                }

                if (!IsValidGeneration(source.Type, command.Type))
                {
                    context.AddFailure(GetErrorMessage(source.Type));
                }
            });
    }

    private static bool IsValidGeneration(DocumentType source, DocumentType target) => source switch
    {
        DocumentType.Quote => target == DocumentType.Proforma,
        DocumentType.Proforma => target == DocumentType.SalesOrder,
        DocumentType.SalesOrder => false,
        _ => false
    };

    private static string GetErrorMessage(DocumentType source) => source switch
    {
        DocumentType.Quote => "A Quote can only generate a Proforma.",
        DocumentType.Proforma => "A Proforma can only generate a SalesOrder.",
        DocumentType.SalesOrder => "A SalesOrder cannot generate another document.",
        _ => "Invalid document generation request."
    };
}
using Aruba.Document.Domain.Enums;
using Aruba.Document.Domain.Exceptions;
using FluentAssertions;

namespace Aruba.Document.UnitTests.Domain.Models;

public class DocumentTests
{
    [Fact]
    public void Document_Constructor_ShouldCreateDocument_WithValidData()
    {
        // Given-When
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario Rossi", "Test description");

        // Then
        document.Id.Should().NotBeNullOrWhiteSpace();
        document.Type.Should().Be(DocumentType.Quote);
        document.Status.Should().Be(DocumentStatus.Draft);
        document.CustomerName.Should().Be("Mario Rossi");
        document.Description.Should().Be("Test description");
        document.LinkedDocuments.Should().BeEmpty();
        document.CreatedAt.Should().NotBe(default);
        document.UpdatedAt.Should().BeNull();
    }

    [Fact]
    public void Document_Constructor_ShouldThrow_WhenCustomerNameIsNull()
    {
        // Given-When
        Action act = () => new Document.Domain.Models.Document(DocumentType.Quote, null, "desc");

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("CustomerName cannot be empty.");
    }

    [Fact]
    public void Document_Constructor_ShouldThrow_WhenDescriptionIsNull()
    {
        // Given-When
        Action act = () => new Document.Domain.Models.Document(DocumentType.Quote, "Mario", null);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Description cannot be empty.");
    }

    [Fact]
    public void Document_Complete_ShouldSetStatusToCompleted_WhenDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        document.Complete();

        // Then
        document.Status.Should().Be(DocumentStatus.Completed);
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_Complete_ShouldThrow_WhenNotDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();

        // When
        Action act = () => document.Complete();

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Only draft documents can be completed.");
    }

    [Fact]
    public void Document_Send_ShouldSetStatusToSent_WhenCompleted()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();

        // When
        document.Send();

        // Then
        document.Status.Should().Be(DocumentStatus.Sent);
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_Send_ShouldThrow_WhenNotCompleted()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => document.Send();

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Only completed documents can be sent.");
    }

    [Fact]
    public void Document_Approve_ShouldSetStatusToApproved_WhenSent()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        document.Send();

        // When
        document.Approve();

        // Then
        document.Status.Should().Be(DocumentStatus.Approved);
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_Approve_ShouldThrow_WhenNotSent()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => document.Approve();

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Only sent documents can be approved.");
    }

    [Fact]
    public void Document_Reject_ShouldSetStatusToRejected_WhenSent()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        document.Complete();
        document.Send();

        // When
        document.Reject();

        // Then
        document.Status.Should().Be(DocumentStatus.Rejected);
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_Reject_ShouldThrow_WhenNotSent()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => document.Reject();

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Only sent documents can be rejected.");
    }

    [Fact]
    public void Document_ChangeType_ShouldUpdate_WhenDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Old Name", "desc");

        // When
        document.ChangeType(DocumentType.SalesOrder);

        // Then
        document.Type.Should().Be(DocumentType.SalesOrder);
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_ChangeType_ShouldThrow_WhenNotDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Old Name", "desc");
        document.Complete();

        // When
        Action act = () => document.ChangeType(DocumentType.SalesOrder);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Document type can only be changed when the document is in Draft status.");
    }

    [Fact]
    public void Document_UpdateCustomerName_ShouldUpdate_WhenDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Old Name", "desc");

        // When
        document.UpdateCustomerName("New Name");

        // Then
        document.CustomerName.Should().Be("New Name");
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_UpdateCustomerName_ShouldThrow_WhenNotDraft()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Old Name", "desc");
        document.Complete();

        // When
        Action act = () => document.UpdateCustomerName("New Name");

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Customer name can only be changed when the document is in Draft status.");
    }

    [Fact]
    public void Document_UpdateCustomerName_ShouldThrow_WhenNull()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Old Name", "desc");

        // When
        Action act = () => document.UpdateCustomerName(null);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("CustomerName cannot be empty.");
    }

    [Fact]
    public void Document_UpdateDescription_ShouldUpdate()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "Old desc");

        // When
        document.UpdateDescription("New desc");

        // Then
        document.Description.Should().Be("New desc");
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_UpdateDescription_ShouldThrow_WhenNull()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => document.UpdateDescription(null);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Description cannot be empty.");
    }

    [Fact]
    public void Document_LinkTo_ShouldAddLinkedDocument()
    {
        // Given
        var document = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        document.LinkTo("123");

        // Then
        document.LinkedDocuments.Should().Contain("123");
        document.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Document_LinkTo_ShouldThrow_WhenIdIsNull()
    {
        // Given
        var doc = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => doc.LinkTo(null);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Linked document ID cannot be empty.");
    }

    [Fact]
    public void Document_LinkTo_ShouldThrow_WhenIdIsEmpty()
    {
        // Given
        var doc = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => doc.LinkTo("");

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Linked document ID cannot be empty.");
    }

    [Fact]
    public void Document_LinkTo_ShouldThrow_WhenLinkingToSelf()
    {
        // Given
        var doc = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        Action act = () => doc.LinkTo(doc.Id);

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("A document cannot link to itself.");
    }

    [Fact]
    public void Document_LinkTo_ShouldThrow_WhenAlreadyLinked()
    {
        // Given
        var doc = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");
        doc.LinkTo("123");

        // When
        Action act = () => doc.LinkTo("123");

        // Then
        act.Should().Throw<DomainException>()
            .WithMessage("Document already linked.");
    }

    [Fact]
    public void Document_LinkTo_ShouldAddLinkedDocument_WhenValid()
    {
        // Given
        var doc = new Document.Domain.Models.Document(DocumentType.Quote, "Mario", "desc");

        // When
        doc.LinkTo("456");

        // Then
        doc.LinkedDocuments.Should().Contain("456");
        doc.UpdatedAt.Should().NotBeNull();
    }
}
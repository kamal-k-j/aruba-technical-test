using Aruba.Document.Domain.Enums;
using Aruba.Document.Domain.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aruba.Document.Domain.Models;

public class Document : IEntity
{
    [BsonElement("LinkedDocuments")]
    private List<string> _linkedDocuments;

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    public DocumentType Type { get; private set; }

    public DocumentStatus Status { get; private set; }

    public string CustomerName { get; private set; }

    public string Description { get; private set; }

    [BsonIgnore]
    public IReadOnlyList<string> LinkedDocuments => _linkedDocuments;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public Document(DocumentType type, string customerName, string description)
    {
        Id = ObjectId.GenerateNewId().ToString();
        Type = type;
        Status = DocumentStatus.Draft;
        CustomerName = customerName ?? throw new DomainException("CustomerName cannot be empty.");
        Description = description ?? throw new DomainException("Description cannot be empty.");
        CreatedAt = DateTime.Now;
        _linkedDocuments = [];
    }

    public void Complete()
    {
        if (Status is not DocumentStatus.Draft)
        {
            throw new DomainException("Only draft documents can be completed.");
        }

        Status = DocumentStatus.Completed;
        UpdatedAt = DateTime.Now;
    }

    public void Send()
    {
        if (Status is not DocumentStatus.Completed)
        {
            throw new DomainException("Only completed documents can be sent.");
        }

        Status = DocumentStatus.Sent;
        UpdatedAt = DateTime.Now;
    }

    public void Approve()
    {
        if (Status is not DocumentStatus.Sent)
        {
            throw new DomainException("Only sent documents can be approved.");
        }

        Status = DocumentStatus.Approved;
        UpdatedAt = DateTime.Now;
    }

    public void Reject()
    {
        if (Status != DocumentStatus.Sent)
        {
            throw new DomainException("Only sent documents can be rejected.");
        }

        Status = DocumentStatus.Rejected;
        UpdatedAt = DateTime.Now;
    }

    public void ChangeType(DocumentType newDocumentType)
    {
        if (Status is not DocumentStatus.Draft)
        {
            throw new DomainException("Document type can only be changed when the document is in Draft status.");
        }

        Type = newDocumentType;
        UpdatedAt = DateTime.Now;
    }

    public void UpdateCustomerName(string newCustomerName)
    {
        if (Status is not DocumentStatus.Draft)
        {
            throw new DomainException("Customer name can only be changed when the document is in Draft status.");
        }

        CustomerName = newCustomerName ?? throw new DomainException("CustomerName cannot be empty.");
        UpdatedAt = DateTime.Now;
    }

    public void UpdateDescription(string newDescription)
    {
        Description = newDescription ?? throw new DomainException("Description cannot be empty.");
        UpdatedAt = DateTime.Now;
    }

    public void LinkTo(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            throw new DomainException("Linked document ID cannot be empty.");
        }

        if (id == Id)
        {
            throw new DomainException("A document cannot link to itself.");
        }

        if (_linkedDocuments.Contains(id))
        {
            throw new DomainException("Document already linked.");
        }

        _linkedDocuments.Add(id);
        UpdatedAt = DateTime.Now;
    }
}
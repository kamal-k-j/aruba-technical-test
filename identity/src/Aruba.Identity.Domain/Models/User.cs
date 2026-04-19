using Aruba.Identity.Domain.Exceptions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aruba.Identity.Domain.Models;

public class User : IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; }

    public string UserName { get; private set; }

    public string Email { get; private set; }

    public string Address { get; private set; }

    public string PasswordHash { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public User(string userName, string email, string passwordHash, string address = null)
    {
        Id = ObjectId.GenerateNewId().ToString();
        UserName = userName ?? throw new DomainException("Username cannot be empty.");
        Email = email ?? throw new DomainException("Email cannot be empty.");
        PasswordHash = passwordHash ?? throw new DomainException("PasswordHash cannot be empty.");
        Address = address;
        CreatedAt = DateTime.Now;
    }

    public void UpdateEmail(string newEmail)
    {
        Email = newEmail ?? throw new DomainException("Email cannot be empty.");
        UpdatedAt = DateTime.Now;
    }

    public void UpdateAddress(string newAddress)
    {
        Address = newAddress;
        UpdatedAt = DateTime.Now;
    }

    public void ChangePassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash ?? throw new DomainException("PasswordHash cannot be empty.");
        UpdatedAt = DateTime.Now;
    }
}
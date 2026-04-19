using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Aruba.Document.Domain;

public interface IEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; }

    public DateTime CreatedAt { get; }

    public DateTime? UpdatedAt { get; }
}
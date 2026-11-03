using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PetOS.Models;

/// <summary>
/// Representa uma atividade de um pet armazenada no MongoDB.
/// </summary>
public class PetActivity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } =
        ObjectId.GenerateNewId().ToString();

    [BsonElement("petId")]
    public long PetId { get; set; }

    [BsonElement("type")]
    public string Type { get; set; } = string.Empty;

    [BsonElement("description")]
    public string Description { get; set; } = string.Empty;

    [BsonElement("metadata")]
    public Dictionary<string, string> Metadata { get; set; } = new();

    [BsonElement("createdAtUtc")]
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
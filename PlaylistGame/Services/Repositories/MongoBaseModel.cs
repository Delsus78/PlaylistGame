using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace PlaylistGame.Services.Repositories;

public abstract class MongoBaseModel
{
    [BsonId]
    public string Id { get; set; }
}
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    public sealed class ActivityLogDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("operationType")]
        public string OperationType { get; set; }
        = string.Empty;
        [BsonElement("blogPostId")]
        public string? BlogPostId { get; set; }
        [BsonElement("title")]
        public string? Title { get; set; }
        [BsonElement("message")]
        public string Message { get; set; }
        = string.Empty;
        [BsonElement("createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; }
        [BsonElement("expiresAtUtc")]
        public DateTime ExpiresAtUtc { get; set; }
    }
}

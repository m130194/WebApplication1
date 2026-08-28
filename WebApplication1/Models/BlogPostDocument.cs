using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace WebApplication1.Models
{
    //This allows the C# application to read a document even if MongoDB contains additional fields that are not represented by the class.
    [BsonIgnoreExtraElements]
    //MongoDB normally generates an ObjectId for a new document.
    //To avoid breaking the existing Session 2 and Session 3 "BlogPost.cs" code, create a separate document model.
    public sealed class BlogPostDocument
    {
        //This identifies the property that maps to MongoDB’s _id field.
        [BsonId]
        //This allows the application to represent the MongoDB ObjectId as a C# string.
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        
        [BsonElement("title")]
        public string Title { get; set; } = string.Empty;
        [BsonElement("content")]
        public string Content { get; set; } = string.Empty;
        [BsonElement("createdAtUtc")]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}

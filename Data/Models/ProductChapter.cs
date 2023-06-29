using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Data.Models
{
    public class ProductChapter
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("ProductId")]
        public string ProductId { get; set; }

        [BsonElement("Number")]
        public int Number { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Slug")]
        public string Slug { get; set; }

        [BsonElement("IsRemoved")]
        public Boolean IsRemoved { get; set; }

        [BindNever]
        [BsonElement("Created_at")]
        public DateTime Created_at { get; set; }

        [BindNever]
        [BsonElement("Updated_at")]
        public DateTime Updated_at { get; set; }

        [BindNever]
        [BsonElement("Removed_at")]
        public DateTime Removed_at { get; set; }
    }
}

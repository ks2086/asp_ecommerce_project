using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Data.Models
{
    public class Promotion
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Value")]
        public int Value { get; set; }

        [BsonElement("Start_at")]
        public DateTime Start_at { get; set; }

        [BsonElement("Ends_at")]
        public DateTime Ends_at { get; set; }

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

using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Data.Models
{
    public class UserModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Email")]
        public string Email { get; set; }

        [BsonElement("Password")]
        public string Password { get; set; }

        [BindNever]
        [BsonElement("Created_at")]
        public DateTime Created_at { get; set; }

        [BindNever]
        [BsonElement("Updated_at")]
        public DateTime Updated_at { get; set; }
    }
}

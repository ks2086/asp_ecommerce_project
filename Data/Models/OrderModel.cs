using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Data.Models
{
    public class OrderModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("SessionOrderId")]
        public string SessionOrderId { get; set; }

        [BsonElement("AccountId")]
        public string AccountId { get; set; }

        [BsonElement("OrderSum")]
        public decimal OrderSum { get; set; }

        [BindNever]
        [BsonElement("Created_at")]
        public DateTime Created_at { get; set; }

    }
}

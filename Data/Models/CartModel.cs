using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Data.Models
{
    public class CartModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("SessionOrderId")]
        public string SessionOrderId { get; set; }

        [BsonElement("ProductId")]
        public string ProductId { get; set; }

        [BsonElement("ProductTitle")]
        public string ProductTitle { get; set; }

        [BsonElement("UnitPrice")]
        public decimal UnitPrice { get; set; }

        [BindNever]
        [BsonElement("Created_at")]
        public DateTime Created_at { get; set; }
    }
}

using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Data.Models
{
    public class ContentTypesModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Slug")]
        public string Slug { get; set; }
    }
}

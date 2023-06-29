using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Data.Models
{
    public class Product
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        public string Title { get; set; }

        [BsonElement("Slug")]
        public string Slug { get; set; }

        [BsonElement("CategoryId")]
        public string CategoryId { get; set; }

        [BsonElement("ShortDescription")]
        public string ShortDescription { get; set; }
        [BsonElement("Text")]
        public string Text { get; set; }

        [BsonElement("ImageId")]
        public string ImageId { get; set; }

        [BsonElement("PriceNetto")]
        public decimal PriceNetto { get; set; }

        [BsonElement("Tax")]
        public int Tax { get; set; }

        [BsonElement("PriceBrutto")]
        public decimal PriceBrutto { get; set; }

        [BsonElement("DifficultyLevel")]
        public int DifficultyLevel { get; set; }

        [BsonElement("IsBestseller")]
        public Boolean IsBestseller { get; set; }

        [BsonElement("IsPromotion")]
        public Boolean IsPromotion { get; set; }

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
        public ProductImage? Images { get; internal set; }
        public ProductCategory? Categories { get; internal set; }
    }
}

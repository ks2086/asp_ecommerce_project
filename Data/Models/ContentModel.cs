using Microsoft.AspNetCore.Mvc.ModelBinding;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Data.Models
{
    public class ContentModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("Title")]
        [Display(Name = "Podaj tytuł nowej pozycji")]
        public string Title { get; set; }

        [BsonElement("Type")]
        [Display(Name = "Wybierz rodzaj contentu")]
        public string Type { get; set; }

        [BindNever]
        [BsonElement("Slug")]
        public string Slug { get; set; }

        [BsonElement("Text")]
        [Display(Name = "Podaj treść strony")]
        public string Text { get; set; }

        [BsonElement("Boolean")]
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

using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Data.Services
{
    public class ProductChaptersService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductChapter> _itemsCollection;

        public ProductChaptersService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<ProductChapter>("ProductChapters");
        }

    }

}

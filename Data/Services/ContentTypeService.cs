using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Data.Services
{
    public class ContentTypeService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ContentTypesModel> _itemsCollection;

        public ContentTypeService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<ContentTypesModel>("ContentTypes");
        }

        public async Task<ContentTypesModel> GetWhereSlugAsync(string slug)
        {
            var filter = Builders<ContentTypesModel>.Filter.Eq(x => x.Slug, slug);
            var result = await _itemsCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<ContentTypesModel?> GetFirstAsync() => await _itemsCollection.Find(x => true).FirstOrDefaultAsync();

        public async Task<List<ContentTypesModel>> GetListAsync() => await _itemsCollection.Find(_ => true).ToListAsync();

        public async Task CreateAsync(ContentTypesModel newItem) => await _itemsCollection.InsertOneAsync(newItem);
    }
}

using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Services
{
    public class ContentService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ContentModel> _contentCollection;

        public ContentService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _contentCollection = _database.GetCollection<ContentModel>("Content");
        }

        public async Task<List<ContentModel>> GetWhereContentTypeSlugAsync(string slug)
        {
            var filter = Builders<ContentModel>.Filter.And(
                 Builders<ContentModel>.Filter.Eq(x => x.Type, slug),
                 Builders<ContentModel>.Filter.Eq(x => x.IsRemoved, false)
             );
            var result = await _contentCollection.Find(filter).ToListAsync();
            return result;
        }

        public async Task<ContentModel> GetWhereIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ContentModel>.Filter.Eq(x => x.Id, objectId);
            var result = await _contentCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<ContentModel> GetWhereSlugAsync(string slug)
        {
            var filter = Builders<ContentModel>.Filter.Eq(x => x.Slug, slug);
            var result = await _contentCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task CreateAsync(ContentModel newContent) => await _contentCollection.InsertOneAsync(newContent);

        public async Task<Boolean> UpdateAsync(string id, ContentModel updatedItem)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ContentModel>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ContentModel>.Update
                    .Set(x => x.Title, updatedItem.Title)
                    .Set(x => x.Slug, updatedItem.Slug)
                    .Set(x => x.Text, updatedItem.Text)
                    .CurrentDate(x => x.Updated_at);

            var result = await _contentCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Boolean> RemoveAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ContentModel>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ContentModel>.Update
                    .Set(x => x.IsRemoved, true)
                    .CurrentDate(x => x.Removed_at);

            var result = await _contentCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }
    }
}

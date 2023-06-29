using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Services
{
    public class PromotionService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Promotion> _itemsCollection;

        public PromotionService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<Promotion>("Promotions");
        }

        public async Task<List<Promotion>> GetListAsync()
        {
            var filter = Builders<Promotion>.Filter.Eq(x => x.IsRemoved, false);
            var result = await _itemsCollection.Find(filter).ToListAsync();
            return result;
        }
        public async Task CreateAsync(Promotion newContent) => await _itemsCollection.InsertOneAsync(newContent);

        public async Task<Promotion> GetWhereIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Promotion>.Filter.Eq(x => x.Id, objectId);
            var result = await _itemsCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task UpdateAsync(string id, Promotion updatedItem)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Promotion>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<Promotion>.Update
                    .Set(x => x.Title, updatedItem.Title)
                    .Set(x => x.Value, updatedItem.Value)
                    .Set(x => x.Start_at, updatedItem.Start_at)
                    .Set(x => x.Ends_at, updatedItem.Ends_at)
     
                    .CurrentDate(x => x.Updated_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);
        }

        public async Task DeleteAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Promotion>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<Promotion>.Update
                    .Set(x => x.IsRemoved, true)
                    .CurrentDate(x => x.Removed_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);
        }

        public async Task<Promotion> GetCurrentPromotion()
        {
            var currentDate = DateTime.Now.Date;
            var filter = Builders<Promotion>.Filter.And(
                Builders<Promotion>.Filter.Eq(x => x.IsRemoved, false),
                Builders<Promotion>.Filter.Lte(x => x.Start_at, currentDate),
                Builders<Promotion>.Filter.Gte(x => x.Ends_at, currentDate)
            );
           return await _itemsCollection.Find(filter).FirstOrDefaultAsync(); 
            
        }

    }
}

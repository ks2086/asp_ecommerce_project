using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Data.Services
{
    public class ProductImagesService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductImage> _itemsCollection;

        public ProductImagesService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<ProductImage>("ProductImages");
        }

        public async Task<List<ProductImage>> GetListAsync()
        {

            var filter = Builders<ProductImage>.Filter.Eq(x => x.IsRemoved, false);
            var result = await _itemsCollection.Find(filter).ToListAsync();
            return result;
        }

        public async Task CreateAsync(ProductImage newContent) => await _itemsCollection.InsertOneAsync(newContent);

        public async Task<ProductImage> GetWhereIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductImage>.Filter.Eq(x => x.Id, objectId);
            var result = await _itemsCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<Boolean> UpdateAsync(string id, ProductImage updatedItem)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductImage>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ProductImage>.Update
                    .Set(x => x.Title, updatedItem.Title)
                    .Set(x => x.Slug, updatedItem.Slug)
                    .Set(x => x.Url, updatedItem.Url)
                    .CurrentDate(x => x.Updated_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Boolean> RemoveAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductImage>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ProductImage>.Update
                    .Set(x => x.IsRemoved, true)
                    .CurrentDate(x => x.Removed_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }


    }
}
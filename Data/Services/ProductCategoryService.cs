using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System;

namespace Data.Services
{
    public class ProductCategoryService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<ProductCategory> _itemsCollection;

        public ProductCategoryService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<ProductCategory>("ProductCategory");
        }

        public async Task<List<ProductCategory>> GetListAsync()
        {

            var filter = Builders<ProductCategory>.Filter.Eq(x => x.IsRemoved, false);
            var result = await _itemsCollection.Find(filter).ToListAsync();
            return result;
        }

        public async Task CreateAsync(ProductCategory newContent) => await _itemsCollection.InsertOneAsync(newContent);

        public async Task<ProductCategory> GetWhereIdAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductCategory>.Filter.Eq(x => x.Id, objectId);
            var result = await _itemsCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<ProductCategory> GetWhereSlugAsync(string slug)
        {
            var filter = Builders<ProductCategory>.Filter.Eq(x => x.Slug, slug);
            var result = await _itemsCollection.Find(filter).FirstAsync();
            return result;
        }

        public async Task<Boolean> UpdateAsync(string id, ProductCategory updatedItem)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductCategory>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ProductCategory>.Update
                    .Set(x => x.Title, updatedItem.Title)
                    .Set(x => x.Slug, updatedItem.Slug)
                    .CurrentDate(x => x.Updated_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Boolean> RemoveAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<ProductCategory>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<ProductCategory>.Update
                    .Set(x => x.IsRemoved, true)
                    .CurrentDate(x => x.Removed_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }


    }
}
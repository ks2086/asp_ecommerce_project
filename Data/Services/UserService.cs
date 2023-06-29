using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Services
{
    public class UserService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UserModel> _itemsCollection;

        public UserService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<UserModel>("Users");
        }

        public async Task<UserModel> GetActiveUserAsync(string userId)
        {
            var objectId = ObjectId.Parse(userId);
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(x => x.Id, objectId)
            );
            return await _itemsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<UserModel> GetUserWhereEmailAsync(string username)
        {
            var filter = Builders<UserModel>.Filter.And(
                Builders<UserModel>.Filter.Eq(x => x.Email, username)
            );
            return await _itemsCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<bool> StoreUserAsync(UserModel user) 
        {
            try
            {
                await _itemsCollection.InsertOneAsync(user);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
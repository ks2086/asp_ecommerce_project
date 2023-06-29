using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Data.Services
{
    public class OrderService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<CartModel> _cartCollection;
        private readonly IMongoCollection<OrderModel> _orderCollection;

        public OrderService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _cartCollection = _database.GetCollection<CartModel>("Cart");
            _orderCollection = _database.GetCollection<OrderModel>("Orders");
        }

        public async Task AddToCartAsync(CartModel newCartItem) => await _cartCollection.InsertOneAsync(newCartItem);

        public async Task<CartModel> GetWhereProductId(string productId, string cartSessionId)
        {
            var filter = Builders<CartModel>.Filter.And(
                Builders<CartModel>.Filter.Eq(x => x.ProductId, productId),
                Builders<CartModel>.Filter.Eq(x => x.SessionOrderId, cartSessionId)
            );
            return await _cartCollection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<List<CartModel>> GetWhereSessionIdListAsync(string cartSessionId)
        {
            var filter = Builders<CartModel>.Filter.And(
               Builders<CartModel>.Filter.Eq(x => x.SessionOrderId, cartSessionId)
            );
            List<CartModel> list = await _cartCollection.Find(filter).ToListAsync();
            return list;
        }

        public async Task<int> CountWhereSessionIdAsync(string cartSessionId)
        {
            List<CartModel> list = await GetWhereSessionIdListAsync(cartSessionId);
            return list != null ? list.Count : 0;
        }

        public async Task DeleteWhereProductIdAsync(string id, string cartSessionId)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<CartModel>.Filter.And(
                Builders<CartModel>.Filter.Eq(x => x.Id, objectId),
                Builders<CartModel>.Filter.Eq(x => x.SessionOrderId, cartSessionId)
            );

            await _cartCollection.DeleteOneAsync(filter);
        }

        public async Task<bool> MakeOrderAsync(OrderModel order)
        {
            try
            {
                await _orderCollection.InsertOneAsync(order);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<OrderModel>> GetUserOrdersAsync(string userId)
        {
            var filter = Builders<OrderModel>.Filter.And(
                Builders<OrderModel>.Filter.Eq(x => x.AccountId, userId)
            );
            return await _orderCollection.Find(filter).ToListAsync();
        }
    }
}
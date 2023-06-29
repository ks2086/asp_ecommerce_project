using Data.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using static System.Net.Mime.MediaTypeNames;

namespace Data.Services
{
    public class ProductService
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Product> _itemsCollection;
        private readonly IMongoCollection<ProductImage> _imagesCollection;
        private readonly IMongoCollection<ProductCategory> _categoriesCollection;

        private readonly ILogger<ProductService> _logger;


        public ProductService(IOptions<ProjectDatabaseSettings> projectDatabaseSettings, ILogger<ProductService> logger)
        {
            var mongoClient = new MongoClient("mongodb+srv://kamilslusar:IOPlTME5XtpQrQmX@cluster-kb.r3ljfu9.mongodb.net/?retryWrites=true&w=majority");
            _database = mongoClient.GetDatabase("apkiInt");

            _itemsCollection = _database.GetCollection<Product>("Products");
            _imagesCollection = _database.GetCollection<ProductImage>("ProductImages");
            _categoriesCollection = _database.GetCollection<ProductCategory>("ProductCategory");
            _logger = logger;
        }

        public async Task<List<Product>> GetListAsync()
        {
            var filter = Builders<Product>.Filter.Eq(x => x.IsRemoved, false);
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetSearchListAsync(string searchQuery)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.IsRemoved, false);
            if (!string.IsNullOrEmpty(searchQuery))
            {
                var regex = new BsonRegularExpression(searchQuery, "i"); // "i" oznacza ignorowanie wielkości liter
                filter &= Builders<Product>.Filter.Regex(x => x.Title, regex);
            }
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetLimitedListAsync(int limit)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.IsRemoved, false);
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).Limit(limit).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetFilteredAsync(string categoryID = null, int defficultyLevel = 0, decimal priceFrom = 0, decimal priceTo = 0)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsRemoved, false),
                categoryID != null ? Builders<Product>.Filter.Eq(x => x.CategoryId, categoryID) : Builders<Product>.Filter.Empty,
                defficultyLevel > 0 ? Builders<Product>.Filter.Eq(x => x.DifficultyLevel, defficultyLevel) : Builders<Product>.Filter.Empty,
                priceTo > 0 ? Builders<Product>.Filter.Gte(x => x.PriceBrutto, priceFrom) & Builders<Product>.Filter.Lte(x => x.PriceBrutto, priceTo) : Builders<Product>.Filter.Empty
            );
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetBestsellersListAsync()
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsRemoved, false),
                Builders<Product>.Filter.Eq(x => x.IsBestseller, true)
            );

            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetPromotionsListAsync()
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsRemoved, false),
                Builders<Product>.Filter.Eq(x => x.IsPromotion, true)
            );
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
        }

        public async Task<List<Product>> GetWhereCategoryIdAsync(string categoryId)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsRemoved, false),
                Builders<Product>.Filter.Eq(x => x.CategoryId, categoryId)
            );
            var sort = Builders<Product>.Sort.Descending(x => x.Created_at);
            var products = await _itemsCollection.Find(filter).Sort(sort).ToListAsync();
            return await _manageProductsOperatations(products);
           
        }

        private async Task<List<Product>> _manageProductsOperatations(List<Product> products)
        {
            var imageIds = products.Select(p => ObjectId.Parse(p.ImageId)).ToList();
            var filterImages = Builders<ProductImage>.Filter.In(x => x.Id, imageIds);
            var images = await _imagesCollection.Find(filterImages).ToListAsync();

            var categoriesIds = products.Select(p => ObjectId.Parse(p.CategoryId)).ToList();
            var filterCategories = Builders<ProductCategory>.Filter.In(x => x.Id, categoriesIds);
            var categories = await _categoriesCollection.Find(filterCategories).ToListAsync();

            foreach (var product in products)
            {
                var objectImageId = ObjectId.Parse(product.ImageId);
                product.Images = images.FirstOrDefault(i => i.Id == objectImageId);

                var objectCategoryId = ObjectId.Parse(product.CategoryId);
                product.Categories = categories.FirstOrDefault(i => i.Id == objectCategoryId);
            }

            return products;
        }



        public async Task CreateAsync(Product newContent) => await _itemsCollection.InsertOneAsync(newContent);

        private async Task<Product> _manageSingleProductOperatations(Product product)
        {
            if (product != null)
            {
                var imageId = ObjectId.Parse(product.ImageId);
                var imageFilter = Builders<ProductImage>.Filter.Eq(x => x.Id, imageId);
                product.Images = await _imagesCollection.Find(imageFilter).FirstOrDefaultAsync();

                var categoryId = ObjectId.Parse(product.CategoryId);
                var categoryFilter = Builders<ProductCategory>.Filter.Eq(x => x.Id, categoryId);
                product.Categories = await _categoriesCollection.Find(categoryFilter).FirstOrDefaultAsync();
            }

            return product;
        }

        public async Task<Product> GetWhereSlugAsync(string slug)
        {
            var filter = Builders<Product>.Filter.And(
                Builders<Product>.Filter.Eq(x => x.IsRemoved, false),
                Builders<Product>.Filter.Eq(x => x.Slug, slug)
            );
            var product = await _itemsCollection.Find(filter).FirstOrDefaultAsync();
            return await _manageSingleProductOperatations(product);


        }

        public async Task<Product> GetWhereIdAsync(string productId)
        {
            var filter = Builders<Product>.Filter.Eq(x => x.Id, ObjectId.Parse(productId));
            var product = await _itemsCollection.Find(filter).FirstOrDefaultAsync();
            return await _manageSingleProductOperatations(product);
        }

        public async Task<Boolean> UpdateAsync(string id, Product updatedItem)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Product>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<Product>.Update
                    .Set(x => x.Title, updatedItem.Title)
                    .Set(x => x.Slug, updatedItem.Slug)
                    .Set(x => x.Text, updatedItem.Text)
                    .Set(x => x.CategoryId, updatedItem.CategoryId)
                    .Set(x => x.ImageId, updatedItem.ImageId)
                    .Set(x => x.PriceNetto, updatedItem.PriceNetto)
                    .Set(x => x.PriceBrutto, updatedItem.PriceBrutto)
                    .Set(x => x.IsBestseller, updatedItem.IsBestseller)
                    .Set(x => x.IsPromotion, updatedItem.IsPromotion)
                    .Set(x => x.ShortDescription, updatedItem.ShortDescription)
                    .Set(x => x.DifficultyLevel, updatedItem.DifficultyLevel)
                    .Set(x => x.Tax, updatedItem.Tax)

                    .CurrentDate(x => x.Updated_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

        public async Task<Boolean> RemoveAsync(string id)
        {
            var objectId = ObjectId.Parse(id);
            var filter = Builders<Product>.Filter.Eq(x => x.Id, objectId);
            var update = Builders<Product>.Update
                    .Set(x => x.IsRemoved, true)
                    .CurrentDate(x => x.Removed_at);

            var result = await _itemsCollection.UpdateOneAsync(filter, update);

            return result.ModifiedCount > 0;
        }

    }
     
}

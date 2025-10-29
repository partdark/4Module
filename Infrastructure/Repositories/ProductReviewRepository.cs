using Domain.Interfaces;
using Domain.Entitties;
using MongoDB.Driver;
using MongoDB.Bson;


namespace Repository
{
    public class ProductReviewRepository : IProductReviewRepository
    {

        private readonly IMongoCollection<ProductReview> _reviews;
        public ProductReviewRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("bookdb");
            _reviews = database.GetCollection<ProductReview>("reviews");
        }
      public  async Task<ProductReview> CreateAsync(ProductReview productReview)
        {
            await _reviews.InsertOneAsync(productReview);
            return productReview;
        }

        public async Task<IEnumerable<ProductReview>> GetAllAsync()
        {
            return await _reviews.Find(_ => true).ToListAsync();
        }

        public async Task<ProductReview> GetByIdAsync(string id)
        {
            return await _reviews.Find(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ProductReview>> GetByProductAsync(Guid id)
        {
           return await _reviews.Find(r => r.BookId == id).ToListAsync();
        }

        
    }
}

using Domain.Entitties;

namespace Domain.Interfaces
{
    public  interface IProductReviewRepository
    {
       public Task<ProductReview> CreateAsync(ProductReview productReview);
        public Task<ProductReview> GetByIdAsync(string id);

        public Task<IEnumerable<ProductReview>> GetByProductAsync(Guid id);
        
    }
}

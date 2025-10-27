using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;
using Domain.Interfaces;

namespace Application.Services
{
    public class ProductReviewService : IProductReviewService
    {

        private readonly IProductReviewRepository _productReviewRepository;
        public ProductReviewService(IProductReviewRepository productReviewRepository)
        {
            _productReviewRepository = productReviewRepository;
        }


        public async Task<ProductReviewResponseDTO> CreateAsync(CreateProductReviewDTO productReviewDto)
        {
            var productReview = new ProductReview
            {
                BookId = productReviewDto.BookId,
                ReviewerName = productReviewDto.ReviewerName,
                Rating = productReviewDto.Rating,
                Comment = productReviewDto.Comment


            };

            await _productReviewRepository.CreateAsync(productReview);
            return MapToDto(productReview);
        }

        public async Task<ProductReviewResponseDTO?> GetByIdAsync(string id)
        {
            var review = await _productReviewRepository.GetByIdAsync(id);
            return review == null ? null : MapToDto(review);
        }

        public async Task<IEnumerable<ProductReviewResponseDTO>> GetByProductAsync(Guid id)
        {
            var reviews = await _productReviewRepository.GetByProductAsync(id);

            return reviews.Select(MapToDto);

        }


        private ProductReviewResponseDTO MapToDto(ProductReview review)
        {
            return new ProductReviewResponseDTO(
                review.Id,
                review.BookId,
                review.ReviewerName,
                review.Rating,
                review.Comment,
                review.Date
            );
        }
    }


}

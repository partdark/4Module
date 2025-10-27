using Application.DTO;

namespace Application.Interfaces
{
    public interface IProductReviewService
    {
        Task<ProductReviewResponseDTO> CreateAsync(CreateProductReviewDTO productReviewDto);
        Task<ProductReviewResponseDTO?> GetByIdAsync(string id);
        Task<IEnumerable<ProductReviewResponseDTO>> GetByProductAsync(Guid id);
    }
}
using _4Module.DTO;

namespace _4Module.Repository
{
    public interface IBookRepository
    {
        Task<BookResponseDTO> CreateAsync(CreateBookDTO bookDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<BookResponseDTO>> GetAllAsync();
        Task<IEnumerable<BookResponseDTO>> GetBooksByAuthorIdAsync(Guid authorId);
        Task<BookResponseDTO?> GetByIdAsync(Guid id);
        Task<BookResponseDTO?> UpdateAsync(UpdateBookDTO bookDto);
        Task<bool> CreateBookWithAuthorAsync(CreateBookWithAuthorDTO dto);


    }
}
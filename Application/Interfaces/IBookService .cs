





using Application.DTO;

namespace Application
{
    public interface IBookService
    {
        Task<BookResponseDTO> CreateAsync(CreateBookDTO bookDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<BookResponseDTO>> GetAllAsync();
        Task<IEnumerable<BookResponseDTO>> GetBooksByAuthorIdAsync(Guid authorId);
        Task<BookResponseDTO?> GetByIdAsync(Guid id);
        Task<BookResponseDTO?> UpdateAsync(UpdateBookDTO bookDto);

        Task<bool> CreateBookWithAuthorAsync(CreateBookWithAuthorDTO dto);

        Task<string> PublicGet();
    }
}
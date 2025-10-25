using _4Module.DTO;

namespace _4Module.Repository
{
    public interface IAuthorRepository
    {
        Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO authorDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<AuthorResponseDTO>> GetAllAsync();
        Task<IEnumerable<AuthorResponseDTO>> GetAuthorsByBookIdAsync(Guid bookId);
        Task<AuthorResponseDTO?> GetByIdAsync(Guid id);
        Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO authorDto);
    }
}
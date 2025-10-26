

using Application.DTO;

namespace Application
{
    public interface IAuthorService
    {
        Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO authorDto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<AuthorResponseDTO>> GetAllAsync();
        Task<AuthorResponseDTO?> GetByIdAsync(Guid id);
        Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO authorDto);

       // Task<IEnumerable<AuthorBookCountDTO>> GetAuthorBookCountsAsync(); перенесен в iauthorreportsevice

    }
}
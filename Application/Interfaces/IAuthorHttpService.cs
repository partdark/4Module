using Application.DTO;
using Domain.Entitties;

namespace Application.Interfaces
{
    public interface IAuthorHttpService
    {
        Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<AuthorResponseDTO>> GetAllAsync();
        Task<AuthorResponseDTO?> GetByIdAsync(Guid id);
        Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO dto);
        Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids);
    }
}
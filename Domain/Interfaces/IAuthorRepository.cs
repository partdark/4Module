

using Domain.Entitties;

namespace Domain.Interfaces
{
    public interface IAuthorRepository
    {
        Task<Author?> GetByIdAsync(Guid id);
        Task<IEnumerable<Author>> GetAllAsync();
        Task<Author> CreateAsync(Author author);
        Task<Author?> UpdateAsync(Author author);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids);
        //  Task<IEnumerable<Author>> GetAuthorBookCountsAsync(); перенесен
    }
}
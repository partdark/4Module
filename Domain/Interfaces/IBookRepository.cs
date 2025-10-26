

using Domain.Entitties;

namespace Domain
{
    public interface IBookRepository
    {
        Task<Book?> GetByIdAsync(Guid id);
        Task<IEnumerable<Book>> GetAllAsync();
        Task<Book> CreateAsync(Book book);
        Task<Book?> UpdateAsync(Book book);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId);
        Task<Book> CreateBookWithAuthorAsync(Book book, Author author);
    }
}
using Domain;
using Domain.Entitties;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly BookContext _context;

        public BookRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Book>> GetAllAsync()
        {
            return await _context.Books
                .Include(b => b.Authors)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Book> CreateAsync(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book?> UpdateAsync(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<Book>> GetBooksByAuthorIdAsync(Guid authorId)
        {
            return await _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Authors.Any(a => a.Id == authorId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Authors
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }

       

      public async Task<Book> CreateBookWithAuthorAsync(Book book, Author author)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Authors.Add(author);
                book.Authors = new List<Author> { author };
                _context.Books.Add(book);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return book;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
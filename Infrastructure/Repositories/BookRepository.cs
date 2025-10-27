using Domain;
using Domain.Entitties;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using System.Text.Json;

namespace Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly BookContext _context;

        private readonly IDistributedCache _cache;

        public BookRepository(BookContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _cache = distributedCache;
        }

        public async Task<Book?> GetByIdAsync(Guid id)
        {
            var cacheKey = $"book:{id}";

            var chachedBook = await _cache.GetStringAsync(cacheKey);
            if (cacheKey != null && !string.IsNullOrEmpty(chachedBook)) {
                return JsonSerializer.Deserialize<Book>(chachedBook);
            }

            var book =  await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book != null) {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                };
                await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(book), options);
                   
            }
            return book;
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
            await _cache.RemoveAsync($"book:{book.Id}");
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return false;
            await _cache.RemoveAsync($"book:{book.Id}");
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
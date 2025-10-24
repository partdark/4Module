using _4Module.Data;
using _4Module.DTO;
using _4Module.Models;
using Microsoft.EntityFrameworkCore;

namespace _4Module.Services
{
    public class BookService : IBookService
    {
        private readonly BookContext _context;

        public BookService(BookContext context)
        {
            _context = context;
        }

        public async Task<BookResponseDTO?> GetByIdAsync(Guid id)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Id == id)
                .Select(b => new BookResponseDTO(
                    b.Id,
                    b.Year,
                    b.Authors.Select(a => a.Id).ToList(),
                    b.Title,
                    b.Authors.Select(a => new AuthorDTO(a.Id, a.Name)).ToList()
                ))
                .FirstOrDefaultAsync();

            return book;
        }

        public async Task<IEnumerable<BookResponseDTO>> GetAllAsync()
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .Select(b => new BookResponseDTO(
                    b.Id,
                    b.Year,
                    b.Authors.Select(a => a.Id).ToList(),
                    b.Title,
                    b.Authors.Select(a => new AuthorDTO(a.Id, a.Name)).ToList()
                ))
                .ToListAsync();

            return books;
        }

        public async Task<BookResponseDTO> CreateAsync(CreateBookDTO bookDto)
        {

            var authors = await _context.Authors
                .Where(a => bookDto.AuthorIds.Contains(a.Id))
                .ToListAsync();

            var book = new Book
            {
                Title = bookDto.Title,
                Year = bookDto.Year,
                Authors = authors
            };

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(book.Id);
        }

        public async Task<BookResponseDTO?> UpdateAsync(UpdateBookDTO bookDto)
        {
            var book = await _context.Books
                .Include(b => b.Authors)
                .FirstOrDefaultAsync(b => b.Id == bookDto.Id);

            if (book == null)
                return null;


            book.Title = bookDto.Title;
            book.Year = bookDto.Year;


            var authors = await _context.Authors
                .Where(a => bookDto.AuthorIds.Contains(a.Id))
                .ToListAsync();

            book.Authors.Clear();
            foreach (var author in authors)
            {
                book.Authors.Add(author);
            }

            await _context.SaveChangesAsync();
            return await GetByIdAsync(book.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
                return false;

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<IEnumerable<BookResponseDTO>> GetBooksByAuthorIdAsync(Guid authorId)
        {
            var books = await _context.Books
                .Include(b => b.Authors)
                .Where(b => b.Authors.Any(a => a.Id == authorId))
                .Select(b => new BookResponseDTO(
                    b.Id,
                    b.Year,
                    b.Authors.Select(a => a.Id).ToList(),
                    b.Title,
                    b.Authors.Select(a => new AuthorDTO(a.Id, a.Name)).ToList()
                ))
                .ToListAsync();

            return books;
        }
    }
}
using _4Module.Data;
using _4Module.DTO;
using _4Module.Models;
using _4Module.Services;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace _4Module.Repository
{

    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookContext _context;

        public AuthorRepository(BookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorBookCountDTO>> GetAuthorBookCountsAsync()
        {
            var sql = @"select a.""Name"" as AuthorName, COUNT(ba.""BooksId"") as BookCount From ""Authors"" a 
                         LEFT JOIN ""BookAuthors"" ba ON a.""Id"" = ba.""AuthorsId""
                         GROUP BY  a.""Id"", a.""Name"" ORDER BY BookCount DESC";
            using var connection = _context.Database.GetDbConnection();
            return await connection.QueryAsync<AuthorBookCountDTO>(sql);
        }

        public async Task<AuthorResponseDTO?> GetByIdAsync(Guid id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Id == id)
                .Select(a => new AuthorResponseDTO(
                    a.Id,
                    a.Name,
                    a.Bio,
                    a.Books.Select(b => new BookDTO(b.Id, b.Title, b.Year)).ToList()
                )).AsNoTracking()
                .FirstOrDefaultAsync();

            return author;
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAllAsync()
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .Select(a => new AuthorResponseDTO(
                    a.Id,
                    a.Name,
                    a.Bio,
                    a.Books.Select(b => new BookDTO(b.Id, b.Title, b.Year)).ToList()
                )).AsNoTracking()
                .ToListAsync();

            return authors;
        }

        public async Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO authorDto)
        {
            var author = new Author
            {
                Name = authorDto.Name,
                Bio = authorDto.Bio
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(author.Id);
        }

        public async Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO authorDto)
        {
            var existingAuthor = await _context.Authors.FindAsync(authorDto.Id);
            if (existingAuthor == null)
                return null;

            existingAuthor.Name = authorDto.Name;
            existingAuthor.Bio = authorDto.Bio;

            await _context.SaveChangesAsync();
            return await GetByIdAsync(authorDto.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (author == null)
                return false;


            if (author.Books.Any())
                return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAuthorsByBookIdAsync(Guid bookId)
        {
            var authors = await _context.Authors
                .Include(a => a.Books)
                .Where(a => a.Books.Any(b => b.Id == bookId))
                .Select(a => new AuthorResponseDTO(
                    a.Id,
                    a.Name,
                    a.Bio,
                    a.Books.Select(b => new BookDTO(b.Id, b.Title, b.Year)).ToList()
                )).AsNoTracking()
                .ToListAsync();

            return authors;
        }
    }
}


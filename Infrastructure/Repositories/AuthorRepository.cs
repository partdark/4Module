
using Domain;
using Domain.Entitties;
using Infastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Repository
{

    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookContext _context;

        public AuthorRepository(BookContext context)
        {
            _context = context;
        }

        //public async Task<IEnumerable<AuthorBookCountDTO>> GetAuthorBookCountsAsync()
        //{
        //    var sql = @"select a.""Name"" as AuthorName, COUNT(ba.""BooksId"") as BookCount From ""Authors"" a 
        //                 LEFT JOIN ""BookAuthors"" ba ON a.""Id"" = ba.""AuthorsId""
        //                 GROUP BY  a.""Id"", a.""Name"" ORDER BY BookCount DESC";
        //    using var connection = _context.Database.GetDbConnection();
        //    return await connection.QueryAsync<AuthorBookCountDTO>(sql);
        //}


        public async Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            return await _context.Authors
                .Where(a => ids.Contains(a.Id))
                .ToListAsync();
        }

        public async Task<Author?> GetByIdAsync(Guid id)
        {
            return await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Author>> GetAllAsync()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Author> CreateAsync(Author author)
        {
            _context.Authors.Add(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<Author?> UpdateAsync(Author author)
        {
            _context.Authors.Update(author);
            await _context.SaveChangesAsync();
            return author;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return false;

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();
            return true;
        }

       
    }
}
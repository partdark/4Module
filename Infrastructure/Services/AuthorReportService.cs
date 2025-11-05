
using Application.DTO;
using Dapper;
using Infrastructure.Data;
using Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;




namespace Infrastructure.Services
{
    public class AuthorReportService : IAuthorReportService
    {
        private readonly BookContext _context;

        public AuthorReportService(BookContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AuthorBookCountDTO>> GetAuthorBookCountsAsync()
        {
            var sql = @"
            SELECT a.""Name"" as AuthorName, COUNT(ba.""BooksId"") as BookCount 
            FROM ""Authors"" a
            LEFT JOIN ""BookAuthors"" ba ON a.""Id"" = ba.""AuthorsId""
            GROUP BY a.""Id"", a.""Name""";

            using var connection = _context.Database.GetDbConnection();
            return await connection.QueryAsync<AuthorBookCountDTO>(sql);
        }
    }
}

using Domain.Entitties;
using Infastructure.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;

namespace IntegrationTest
{

   // api/Book/books
    public class BookIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public BookIntegrationTest(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task BookGetTestCode()
        {
            var response = await _httpClient.GetAsync("/api/Book/books");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    public class BookIntegrationTestInMemory : IClassFixture<MyTestFactory>
    {
        private readonly MyTestFactory _factory;
        private readonly HttpClient _httpClient;

        public BookIntegrationTestInMemory(MyTestFactory factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }

        [Fact]
        public async Task BookGetTestCode()
        {
            var response = await _httpClient.GetAsync("/api/Book/books");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }


    public class MyTestFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
              
                var descriptors = services
                    .Where(d => d.ServiceType.Name.Contains("DbContext") ||
                               d.ServiceType == typeof(DbContextOptions<BookContext>) ||
                               d.ServiceType == typeof(BookContext))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }

              
                services.AddDbContext<BookContext>(options =>
                {
                    options.UseInMemoryDatabase("DbTest");
                }, ServiceLifetime.Scoped);
            });

            var host = base.CreateHost(builder);

            
            InitializeDatabase(host);

            return host;
        }

        private void InitializeDatabase(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BookContext>();

            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            InitializeDbForTests(db);
        }


        private void InitializeDbForTests(BookContext db)
        {
            db.Books.RemoveRange(db.Books);
            db.Authors.RemoveRange(db.Authors);
            db.SaveChanges();


            var author1 = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Test Author 1",
                Bio = "Bio for Author 1"
            };

            var author2 = new Author
            {
                Id = Guid.NewGuid(),
                Name = "Test Author 2",
                Bio = "Bio for Author 2"
            };


            var book1 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 1",
                Year = 2020,
                Authors = new List<Author> { author1, author2 }
            };

            var book2 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 2",
                Year = 2021,
                Authors = new List<Author> { author1 }
            };

            var book3 = new Book
            {
                Id = Guid.NewGuid(),
                Title = "Test Book 3",
                Year = 2022,
                Authors = new List<Author> { author2 }
            };


            db.Authors.AddRange(author1, author2);
            db.Books.AddRange(book1, book2, book3);
            db.SaveChanges();
        }
    }
}
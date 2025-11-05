using Domain.Entitties;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace IntegrationTest
{
    public class PostgresTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private PostgreSqlContainer _postgresContainer;


        async Task IAsyncLifetime.InitializeAsync()
        {
            _postgresContainer = new PostgreSqlBuilder()
                 .WithDatabase("testdb")
                 .WithUsername("testuser")
                 .WithPassword("testpassword")
                 .Build();
            await _postgresContainer.StartAsync();
        }

        async Task IAsyncLifetime.DisposeAsync()
        {
            if (_postgresContainer != null)
            {
                await _postgresContainer.DisposeAsync();
            }
        }


        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {

                var authServices = services.Where(s =>
                s.ServiceType.FullName.Contains("Authentication") ||
                s.ServiceType == typeof(IAuthenticationService) ||
                s.ServiceType == typeof(IAuthenticationSchemeProvider) ||
                s.ServiceType.Name.Contains("JwtBearer") ||
                s.ServiceType.Name.Contains("AuthenticationOptions"))
                .ToList();

                foreach (var service in authServices)
                {
                    services.Remove(service);
                }


                services.AddAuthentication("Bearer")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Bearer", options => { });



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
                    options.UseNpgsql(_postgresContainer.GetConnectionString());
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


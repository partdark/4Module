using Application.Interfaces;
using Domain.Entitties;
using Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using OrderWorkerService.Data;

namespace IntegrationTest
{
    public class MyTestFactory : WebApplicationFactory<Program>
    {
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
                var httpServiceDescriptor = services.FirstOrDefault(d => d.ServiceType == typeof(IAuthorHttpService));
                if (httpServiceDescriptor != null)
                {
                    services.Remove(httpServiceDescriptor);
                }

                services.AddScoped<IAuthorHttpService, MockAuthorHttpService>();


                var descriptors = services
                    .Where(d => d.ServiceType.Name.Contains("DbContext") ||
                               d.ServiceType == typeof(DbContextOptions<BookContext>) ||
                               d.ServiceType == typeof(BookContext))
                    .ToList();

                foreach (var descriptor in descriptors)
                {
                    services.Remove(descriptor);
                }
                var orderDescriptors = services
    .Where(d => d.ServiceType == typeof(DbContextOptions<OrderContext>) ||
               d.ServiceType == typeof(OrderContext))
    .ToList();

                foreach (var descriptor in orderDescriptors)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<OrderContext>(options =>
                {
                    options.UseInMemoryDatabase("OrderDbTest");
                }, ServiceLifetime.Scoped);
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


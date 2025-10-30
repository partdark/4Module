using Application.DTO;
using Domain.Entitties;
using Infastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Testcontainers.PostgreSql;
using Xunit;

namespace IntegrationTest
{
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

        private HttpClient CreateClientWithAuth()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "fake-jwt-token");
            return client;
        }

        private HttpClient CreateClientWithoutAuth()
        {
            return _factory.CreateClient();
        }

        [Fact]
        public async Task BookGetTestCode()
        {
            var _httpClient = CreateClientWithoutAuth();
            var response = await _httpClient.GetAsync("/api/Book/books");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostTestInvalidDto()
        {
            var _httpClient = CreateClientWithoutAuth();
            var badDto = new CreateBookDTO(1200, new List<Guid>(), "test title");
            var response = await _httpClient.PostAsJsonAsync("/api/Book/books", badDto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostTestValidDto()
        {
            var _httpClient = CreateClientWithoutAuth();
            var validDto = new CreateBookDTO(1952, new List<Guid>(), "test title");
            var response = await _httpClient.PostAsJsonAsync("/api/Book/books", validDto);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task AuthTestWithoutToken()
        {
            var _httpClient = CreateClientWithoutAuth();
            var validDto = new CreateAuthorDTO("Jonny", "bio");
            var response = await _httpClient.PostAsJsonAsync("/api/Book/authors", validDto);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task AuthTestWithToken()
        {
            var _httpClient = CreateClientWithAuth();
            var validDto = new CreateAuthorDTO("Jonny", "bio");
            var response = await _httpClient.PostAsJsonAsync("/api/Book/authors", validDto);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }



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
    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return Task.FromResult(AuthenticateResult.Fail("Missing Authorization header"));
            }

            var authHeader = Request.Headers["Authorization"].ToString();

            if (authHeader.StartsWith("Bearer"))
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "test-user-id"),
                    new Claim(ClaimTypes.Name, "testuser@example.com"),
                    new Claim(ClaimTypes.Role, "User")
                };
                var identity = new ClaimsIdentity(claims, "Bearer");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, "Bearer");

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }

            return Task.FromResult(AuthenticateResult.Fail("Invalid scheme"));
        }
    }

    public class E2ETest : IClassFixture<PostgresTestFactory>, IAsyncLifetime
    {
        private readonly PostgresTestFactory _factory;
        private readonly HttpClient _httpClient;

        public E2ETest(PostgresTestFactory factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient();
        }
        public async Task DisposeAsync()
        {
            await _factory.DisposeAsync();
        }

        public Task InitializeAsync()
        {
            return ((IAsyncLifetime)_factory).InitializeAsync();
        }

        [Fact]
        public async Task PostTest()
        {

            var client = _factory.CreateClient();


            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "fake-jwt-token");

            var authorDto = new CreateAuthorDTO("E2E Author", "E2E Bio");
            var postResponse = await client.PostAsJsonAsync("/api/Book/authors", authorDto);

            var responseContent = await postResponse.Content.ReadAsStringAsync();
            Console.WriteLine($"Create author response: {responseContent}");

        }
    }
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

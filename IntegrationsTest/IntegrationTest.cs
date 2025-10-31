using Application.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;
using static IntegrationTest.BookIntegrationTestInMemory;
using static IntegrationTest.BookIntegrationTestInMemoryCircuitTest;

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

    public class BookIntegrationTestInMemoryCircuitTest : IClassFixture<MyTestFactoryCircuitTest>
    {
        private readonly MyTestFactoryCircuitTest _factory;
        private readonly HttpClient _httpClient;

        public BookIntegrationTestInMemoryCircuitTest(MyTestFactoryCircuitTest factory)
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

        [Fact]
        public async Task CircuitBreakerTest()
        {

            var client = CreateClientWithAuth();
            var validDto = new CreateBookDTO(1952, new List<Guid> { Guid.NewGuid() }, "test title");


            for (int i = 1; i <= 6; i++)
            {
                var response = await client.PostAsJsonAsync("/api/Book/books", validDto);
                Console.WriteLine($"Request {i}: {response.StatusCode}");

                if (i <= 3)
                {
                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
                else if (i <= 5)
                {

                    Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
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
}


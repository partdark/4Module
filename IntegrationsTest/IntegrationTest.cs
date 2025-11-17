using Application.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using Xunit;


namespace IntegrationTest
{
   
    public class BookIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>
    {
        private readonly IntegrationTestWebAppFactory _factory;
        private readonly HttpClient _httpClient;

        public BookIntegrationTest(IntegrationTestWebAppFactory factory)
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

            var response = await _httpClient.GetAsync("/api/Book/books");
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task PostTestInvalidDto()
        {

            var _httpClient = CreateClientWithAuth();
            var badDto = new CreateBookDTO(1200, new List<Guid>(), "test title");
            var response = await _httpClient.PostAsJsonAsync("/api/Book/books", badDto);
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task PostTestValidDto()
        {

            var _httpClient = CreateClientWithAuth();
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
}

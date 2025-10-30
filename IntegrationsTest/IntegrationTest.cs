using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace IntegrationTest
{

    //api/Book/books
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


}
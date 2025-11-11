using AuthorGrpc.Contracts;
using BookGrpc.Contracts;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class GrpcController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public GrpcController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet("book/{id}")]
    public async Task<IActionResult> GetBookViaGrpc(string id)
    {
        try
        {
            var bookServiceUrl = GetGrpcServiceUrl("BookService", "http://localhost:7001");
            using var channel = GrpcChannel.ForAddress(bookServiceUrl);
            var client = new BookService.BookServiceClient(channel);

            var response = await client.GetBookAsync(new GetBookRequest { Id = id });

            return Ok(new
            {
                Source = "gRPC BookService",
                ServiceUrl = bookServiceUrl,
                Book = new { response.Id, response.Title, response.Year }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { Error = "BookService unavailable", Details = ex.Message });
        }
    }

    [HttpGet("author/{id}")]
    public async Task<IActionResult> GetAuthorViaGrpc(string id)
    {
        try
        {
            var authorServiceUrl = GetGrpcServiceUrl("AuthorService", "http://localhost:7002");
            using var channel = GrpcChannel.ForAddress(authorServiceUrl);
            var client = new AuthorService.AuthorServiceClient(channel);

            var response = await client.GetAuthorAsync(new GetAuthorRequest { Id = id });

            return Ok(new
            {
                Source = "gRPC AuthorService",
                ServiceUrl = authorServiceUrl,
                Author = new
                {
                    response.Id,
                    response.Name,
                    response.Bio,
                    Books = response.Books.Select(b => new { b.Id, b.Title, b.Year })
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(503, new { Error = "AuthorService unavailable", Details = ex.Message });
        }
    }

    private string GetGrpcServiceUrl(string serviceName, string defaultUrl)
    {
    
        var dockerUrl = _configuration[$"GrpcServices:{serviceName}"];
        if (!string.IsNullOrEmpty(dockerUrl))
            return dockerUrl;

       
        var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";

        if (isDocker)
        {
            return serviceName == "BookService"
                ? "http://grpc-book-service:8080"
                : "http://grpc-author-service:8080";
        }

        return defaultUrl;
    }
}

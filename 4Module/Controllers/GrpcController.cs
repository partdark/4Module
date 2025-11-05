using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;

namespace _4Module.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GrpcController : ControllerBase
{
    [HttpGet("book/{id}")]
    public async Task<IActionResult> GetBookViaGrpc(string id)
    {
        try
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7001");
            var client = new BookGrpc.Contracts.BookService.BookServiceClient(channel);

            var response = await client.GetBookAsync(new BookGrpc.Contracts.GetBookRequest { Id = id });

            return Ok(new
            {
                Source = "gRPC BookService",
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
            using var channel = GrpcChannel.ForAddress("https://localhost:7002");
            var client = new AuthorGrpc.Contracts.AuthorService.AuthorServiceClient(channel);

            var response = await client.GetAuthorAsync(new AuthorGrpc.Contracts.GetAuthorRequest { Id = id });

            return Ok(new
            {
                Source = "gRPC AuthorService",
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
}

using Application;
using Application.Interfaces;
using AuthorGrpc.Contracts;
using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcAuthorsService.Services;

public class AuthorServiceImpl : AuthorGrpc.Contracts.AuthorService.AuthorServiceBase
{
    private readonly IAuthorService _authorService;

    public AuthorServiceImpl(IAuthorService authorService)
    {
        _authorService = authorService;
    }

    public override async Task<AuthorGrpc.Contracts.AuthorResponse> GetAuthor(AuthorGrpc.Contracts.GetAuthorRequest request, ServerCallContext context)
    {
        var authorId = Guid.Parse(request.Id);
        var author = await _authorService.GetByIdAsync(authorId);

        if (author == null)
        {
            throw new RpcException(new Status(StatusCode.NotFound, "Author not found"));
        }

     
        var books = new List<AuthorGrpc.Contracts.BookInfo>();
        if (author.Books.Any())
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:7001");
            var bookClient = new BookGrpc.Contracts.BookService.BookServiceClient(channel);

            foreach (var book in author.Books)
            {
                try
                {
                    var bookResponse = await bookClient.GetBookAsync(new BookGrpc.Contracts.GetBookRequest { Id = book.Id.ToString() });
                    books.Add(new AuthorGrpc.Contracts.BookInfo
                    {
                        Id = bookResponse.Id,
                        Title = bookResponse.Title,
                        Year = bookResponse.Year
                    });
                }
                catch (RpcException)
                {
                   
                    books.Add(new AuthorGrpc.Contracts.BookInfo
                    {
                        Id = book.Id.ToString(),
                        Title = book.Title,
                        Year = book.Year
                    });
                }
            }
        }

        return new AuthorGrpc.Contracts.AuthorResponse
        {
            Id = author.Id.ToString(),
            Name = author.Name,
            Bio = author.Bio ?? "",
            Books = { books }
        };
    }
}

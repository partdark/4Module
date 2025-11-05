using Application;
using Application.DTO;

using Grpc.Core;

namespace GrpcBooksService.Services;

public class BookServiceImpl : BookGrpc.Contracts.BookService.BookServiceBase
{
    private readonly IBookService _bookService;
    private readonly ILogger<BookServiceImpl> _logger;

    public BookServiceImpl(IBookService bookService, ILogger<BookServiceImpl> logger)
    {
        _bookService = bookService;
        _logger = logger;
    }

    public override async Task<BookGrpc.Contracts.BookResponse> GetBook(BookGrpc.Contracts.GetBookRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Getting book with ID: {request.Id}");

            if (!Guid.TryParse(request.Id, out var bookId))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, "Invalid book ID format"));
            }

            var book = await _bookService.GetByIdAsync(bookId);

            if (book == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, "Book not found"));
            }

            return new BookGrpc.Contracts.BookResponse
            {
                Id = book.Id.ToString(),
                Title = book.Title,
                Year = book.Year
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting book");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }

    public override async Task<BookGrpc.Contracts.BookResponse> CreateBook(BookGrpc.Contracts.CreateBookRequest request, ServerCallContext context)
    {
        try
        {
            _logger.LogInformation($"Creating book: {request.Title}");

            var authorIds = request.AuthorIds.Select(id =>
            {
                if (!Guid.TryParse(id, out var guid))
                    throw new RpcException(new Status(StatusCode.InvalidArgument, $"Invalid author ID: {id}"));
                return guid;
            }).ToList();

            var createDto = new CreateBookDTO(request.Year, authorIds, request.Title);
            var book = await _bookService.CreateAsync(createDto);

            return new BookGrpc.Contracts.BookResponse
            {
                Id = book.Id.ToString(),
                Title = book.Title,
                Year = book.Year
            };
        }
        catch (RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating book");
            throw new RpcException(new Status(StatusCode.Internal, "Internal server error"));
        }
    }
}

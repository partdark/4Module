using Application;
using Application.DTO;
using Application.Interfaces;
using Confluent.Kafka;
using Domain.Entitties;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using OpenTelemetry;
using OpenTelemetry.Context.Propagation;
using System.Diagnostics;
using System.Net;
using System.Text;


namespace _4Module.Controllers
{

    /// <summary>
    /// Base Book Controller
    /// </summary>
    [ApiController]

    [Route("api/[controller]")]


    public class BookController : ControllerBase
    {


        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;
        private readonly IAuthorReportService _reportService;
        private readonly IAuthorHttpService _authorHttpService;
        private readonly IProductReviewService _productReview;
        private readonly IDistributedCache _cache;
     

        public BookController(IBookService bookService, IAuthorService authorService, IAuthorReportService reportService
            , IProductReviewService productReview, IDistributedCache cache, IAuthorHttpService authorHttpService)
        {

            _bookService = bookService;
            _authorService = authorService;
            _reportService = reportService;
            _productReview = productReview;
            _cache = cache;
            _authorHttpService = authorHttpService;
           
        }





        /// <summary>
        /// Get all Books
        /// </summary>
        /// <returns>All books</returns>
        /// <response code ="200">Succes</response>

        [HttpGet("books")]

        public async Task<ActionResult<IEnumerable<BookResponseDTO>>> GetAllBooksAsync()
        {
            var books = await _bookService.GetAllAsync();
            if (books == null) { return NotFound(); }
            return Ok(books);
        }
        /// <summary>
        /// Get all aformation about book
        /// </summary>
        /// <returns>Book and reviews</returns>
        /// <response code ="200">Succes</response>
        [HttpGet("products/{id:guid}/details")]
        public async Task<ActionResult<ProductDetailsDto>> GetDetails([FromRoute] Guid id)
        {

            var book = await _bookService.GetByIdAsync(id);
            if (book == null) { return NotFound(); }
            var reviews = await _productReview.GetByProductAsync(id);
            var rating = await _cache.GetStringAsync($"rating:{id}");
            var details = new ProductDetailsDto(book, rating, reviews);

            return Ok(details);
        }


        /// <summary>
        /// Get book by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return book with your id</returns>
        /// <response code ="200">Succes</response>
        /// /// <response code ="404">Book with your id not found</response>
       // [Authorize]
        [HttpGet("books/{id:guid}")]

        public async Task<ActionResult<BookResponseDTO>> GetBookbyId([FromRoute] Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) { return NotFound(); }
            var config = new ProducerConfig { BootstrapServers = "kafka:29092" };
            using var producer = new ProducerBuilder<string, string>(config).Build();

            var headers = new Headers();
            var propagator = Propagators.DefaultTextMapPropagator;
            propagator.Inject(new PropagationContext(Activity.Current?.Context ?? default, Baggage.Current), headers, (h, key, value) => h.Add(key, Encoding.UTF8.GetBytes(value)));

            await producer.ProduceAsync("books-views", new Message<string, string>
            {
                Key = id.ToString(),
                Value = $"Book viewed: {book.Title}",
                Headers = headers
            });
            return Ok(book);
        }

        /// <summary>
        /// Add book 
        /// </summary>
        /// <param name="newBook">DTO</param>
        /// <returns>Created book</returns>
        /// <response code ="400">Data not valid</response>
        [HttpPost("books")]
        public async Task<ActionResult<BookResponseDTO>> CreateBook([FromBody] CreateBookDTO newBook)
        {
            var book = await _bookService.CreateAsync(newBook);
            return Ok(book);
        }


        /// <summary>
        /// Update book with your Id
        /// </summary>
        /// <param name="book">New Data</param>
        [HttpPut("books")]
        public async Task<IActionResult> Update([FromBody] UpdateBookDTO book)
        {
            var UpdatedBook = await _bookService.UpdateAsync(book);
            return Ok(UpdatedBook);

        }

        /// <summary>
        /// Delete book by id
        /// </summary>
        /// <param name="id">Book Id for delete</param>
        [HttpDelete("books/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookById([FromRoute] Guid id)
        {
            await _bookService.DeleteAsync(id);
            return Ok();


        }


        [HttpGet("authors")]
        [Authorize(Policy = "OlderThan18")]
        public async Task<ActionResult<IEnumerable<AuthorResponseDTO>>> GetAuthors()
        {
            var authors = await _authorService.GetAllAsync();
            if (authors == null) { return NotFound(); }
            return Ok(authors);
        }


        [HttpGet("authors/{id:guid}")]
        public async Task<ActionResult<AuthorResponseDTO>> GetAuthor(Guid id)
        {
            var author = await _authorService.GetByIdAsync(id);
            if (author == null)
                return NotFound($"Автор с ID {id} не найден");

            return Ok(author);
        }


        [HttpPost("authors")]
        [Authorize]
        public async Task<ActionResult<AuthorResponseDTO>> CreateAuthor(CreateAuthorDTO authorDto)
        {
            try
            {
                var createdAuthor = await _authorService.CreateAsync(authorDto);
                return StatusCode(201, createdAuthor);
            }
            catch (Exception ex)
            {
                return BadRequest($"Ошибка при создании автора: {ex.Message}");
            }
        }


        [HttpPut("authors/{id:guid}")]
        public async Task<IActionResult> UpdateAuthor([FromRoute] Guid id, [FromBody] UpdateAuthorDTO authorDto)
        {
            if (id != authorDto.Id)
                return BadRequest("ID в пути и в теле запроса не совпадают");

            var updatedAuthor = await _authorService.UpdateAsync(authorDto);
            if (updatedAuthor == null)
                return NotFound($"Автор с ID {id} не найден");

            return NoContent();
        }


        [HttpDelete("authors/{id:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAuthor([FromRoute] Guid id)
        {
            var result = await _authorService.DeleteAsync(id);
            if (!result)
                return BadRequest("Нельзя удалить автора, у которого есть книги");

            return NoContent();
        }

        [HttpPost("bookAndAuthor")]
        public async Task<IActionResult> CreateBookAndAuthor([FromBody] CreateBookWithAuthorDTO dto)
        {
            if (
              await _bookService.CreateBookWithAuthorAsync(dto))
                return Ok();
            else
            {
                return NoContent();

            }
        }
        [HttpGet("GetAuthorBookCountsAsync")]
        public async Task<IEnumerable<AuthorBookCountDTO>> GetAuthorBookCountsAsync()
        {
            return await _reportService.GetAuthorBookCountsAsync();
        }



        [HttpGet("authors/batch")]
        public async Task<ActionResult<IEnumerable<AuthorResponseDTO>>> GetAuthorsByIds([FromQuery] List<Guid> ids)
        {

            var authors = await _authorService.GetByIdsAsync(ids);
            return Ok(authors);
        }
        [HttpGet("test-fallback")]
        public async Task<IActionResult> TestFallback([FromServices] IAuthorHttpService authorHttpService)
        {

            var fakeIds = new List<Guid> { Guid.NewGuid() };
            var authors = await authorHttpService.GetByIdsAsync(fakeIds);
            return Ok(authors);


        }
    }
}


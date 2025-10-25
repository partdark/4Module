using _4Module.DTO;

using _4Module.Models;
using _4Module.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace _4Module.Controllers
{

    /// <summary>
    /// Base Book Controller
    /// </summary>
    [ApiController]
    [Route("[controller]")]


    public class BookController : ControllerBase
    {
        private readonly MySettings _settings;

        private readonly IBookService _bookService;
        private readonly IAuthorService _authorService;


        public BookController(IOptions<MySettings> setting, IBookService bookService, IAuthorService authorService)
        {
            _settings = setting.Value;
            _bookService = bookService;
            _authorService = authorService;
        }


        /// <summary>
        /// Get setting from MySettings
        /// </summary>
        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings()
        {
            await Task.Delay(1000);
            return Ok(new
            {
                ApplicationName = _settings.ApplicationName,
                MaxBooksPerPage = _settings.MaxBooksPerPage,
                ApiSettings = _settings.ApiSettings
            });
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
        /// Get book by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return book with your id</returns>
        /// <response code ="200">Succes</response>
        /// /// <response code ="404">Book with your id not found</response>
        [HttpGet("books/{id:guid}")]

        public async Task<ActionResult<BookResponseDTO>> GetBookbyId([FromRoute] Guid id)
        {
            var book = await _bookService.GetByIdAsync(id);
            if (book == null) { return NotFound(); }
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
        public async Task<IActionResult> DeleteBookById([FromRoute] Guid id)
        {
            await _bookService.DeleteAsync(id);
            return Ok();


        }

        /// <summary>
        /// middleware check
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        [HttpGet("error")]
        public async Task<IActionResult> DivideByZero()
        {
            await Task.Delay(1000);

            throw new InvalidOperationException();
        }
        [HttpGet("authors")]
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
        public async Task<ActionResult<AuthorResponseDTO>> CreateAuthor(CreateAuthorDTO authorDto)
        {
            try
            {
                var createdAuthor = await _authorService.CreateAsync(authorDto);
                return CreatedAtAction(nameof(GetAuthor), new { id = createdAuthor.Id }, createdAuthor);
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
         return   await _authorService.GetAuthorBookCountsAsync();
        }

    }
}


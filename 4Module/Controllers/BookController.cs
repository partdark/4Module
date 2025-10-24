using _4Module.DTO;
using _4Module.Models;
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
        private static readonly LinkedList<Book> _books = new();



        public BookController(IOptions<MySettings> setting)
        {
            _settings = setting.Value;
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
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookResponseDTO>>> Get()
        {
            return Ok();
        }


        /// <summary>
        /// Get book by Id
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns>Return book with your id</returns>
        /// <response code ="200">Succes</response>
        /// /// <response code ="404">Book with your id not found</response>
        [HttpGet("{id:guid}")]

        public async Task<ActionResult<BookResponseDTO>> GetbyId([FromRoute] Guid id)
        {
            return Ok();
        }

        /// <summary>
        /// Add book 
        /// </summary>
        /// <param name="newBook">DTO</param>
        /// <returns>Created book</returns>
        /// <response code ="400">Data not valid</response>
        [HttpPost]
        public async Task<ActionResult<BookResponseDTO>> CreateBook([FromBody] CreateBookDTO newBook)
        {
            return Ok();
        }


        /// <summary>
        /// Update book with your Id
        /// </summary>
        /// <param name="book">New Data</param>
        [HttpPut("id:guid")]
        public async Task<IActionResult> Update([FromBody] UpdateBookDTO book)
        {
            return  Ok(book);   

        }

        /// <summary>
        /// Delete book by id
        /// </summary>
        /// <param name="id">Book Id for delete</param>
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteById([FromRoute] Guid id)
        {
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


    }
}

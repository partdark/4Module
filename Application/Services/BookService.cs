
using Application;
using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;
using Domain.Interfaces;


namespace Applications.Services
{

    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;
        //  private readonly IAuthorRepository _authorRepository; 
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IAuthorHttpService _authorHttpService; 
        private readonly IAnaliticsService _analiticsService;

        public BookService(IBookRepository bookRepository, IAuthorHttpService authorHttpService, IHttpClientFactory httpClientFactory, IAnaliticsService analiticsService)
        {
            _bookRepository = bookRepository;
            _authorHttpService = authorHttpService;
            _httpClientFactory = httpClientFactory;
            _analiticsService = analiticsService;
        }



        public async Task<string> PublicGet()
        {
            var client = _httpClientFactory.CreateClient("TestClient");

            try
            {
                var response = await client.GetAsync("v2/store/inventory/");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                return content;
            }
            catch (Exception ex) {
                throw new Exception($"Ошибка {ex.Message}");
            }
            
        }

        public async Task<BookResponseDTO?> GetByIdAsync(Guid id)
        {
            var book = await _bookRepository.GetByIdAsync(id);
            await _analiticsService.SendEventAsync("books-views", book.Id.ToString(), $"New view to book {book.Title.ToString()}" );
            return book != null ? MapToDto(book) : null;
        }

        public async Task<IEnumerable<BookResponseDTO>> GetAllAsync()
        {
            var books = await _bookRepository.GetAllAsync();
            return books.Select(MapToDto);
        }

        public async Task<BookResponseDTO> CreateAsync(CreateBookDTO dto)
        {

            var authors = await _authorHttpService.GetByIdsAsync(dto.AuthorIds);

            var book = new Book
            {
                Title = dto.Title,
                Year = dto.Year,
                Authors = authors.ToList()
            };

            var created = await _bookRepository.CreateAsync(book);
            await _analiticsService.SendEventAsync("book-create", created.Id.ToString(), $"New book created: {created.Title.ToString()}");
            return MapToDto(created);
        }

        public async Task<BookResponseDTO?> UpdateAsync(UpdateBookDTO dto)
        {
            var authors = await _authorHttpService.GetByIdsAsync(dto.AuthorIds);

            var book = new Book
            {
                Id = dto.Id,
                Title = dto.Title,
                Year = dto.Year,
                Authors = authors.ToList()
            };

            var updated = await _bookRepository.UpdateAsync(book);
            return updated != null ? MapToDto(updated) : null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await (_bookRepository.DeleteAsync(id));
        }


        public async Task<IEnumerable<BookResponseDTO>> GetBooksByAuthorIdAsync(Guid authorId)
        {
            var books = await _bookRepository.GetBooksByAuthorIdAsync(authorId);
            return books.Select(MapToDto);
        }

        public async Task<bool> CreateBookWithAuthorAsync(CreateBookWithAuthorDTO dto)
        {
            var author = new Author
            {
                Name = dto.AuthorName,
                Bio = dto.AuthorBio
            };

            var book = new Book
            {
                Title = dto.BookTitle,
                Year = dto.Year
            };

            try
            {
                await _bookRepository.CreateBookWithAuthorAsync(book, author);
                return true;
            }
            catch
            {
                return false;
            };
        }

        private BookResponseDTO MapToDto(Book book)
        {
            return new BookResponseDTO(
                book.Id,
                book.Year,
                book.Authors.Select(a => a.Id).ToList(),
                book.Title,
                book.Authors.Select(a => new AuthorDTO(a.Id, a.Name)).ToList()
            );
        }
    }
}
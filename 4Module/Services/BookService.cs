using _4Module.Data;
using _4Module.DTO;
using _4Module.Interfaces;
using _4Module.Models;

namespace _4Module.Services
{
    public class BookService : IBookService
    {
        private readonly IBookRepository _bookRepository;

        public BookService(IBookRepository bookRepository)
        {
            _bookRepository = bookRepository;
        }

        public async Task<BookResponseDTO?> GetByIdAsync(Guid id)
        {
            return await _bookRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<BookResponseDTO>> GetAllAsync()
        {
            return await _bookRepository.GetAllAsync();
        }

        public async Task<BookResponseDTO> CreateAsync(CreateBookDTO bookDto)
        {

            return await _bookRepository.CreateAsync(bookDto);
        }

        public async Task<BookResponseDTO?> UpdateAsync(UpdateBookDTO bookDto)
        {
            return await _bookRepository.UpdateAsync(bookDto);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await (_bookRepository.DeleteAsync(id));
        }


        public async Task<IEnumerable<BookResponseDTO>> GetBooksByAuthorIdAsync(Guid authorId)
        {
            return await _bookRepository.GetBooksByAuthorIdAsync(authorId);
        }

        public async Task<bool> CreateBookWithAuthorAsync(CreateBookWithAuthorDTO dto)
        {
            return await _bookRepository.CreateBookWithAuthorAsync(dto);
        }


    }
}
using _4Module.Data;
using _4Module.DTO;
using _4Module.Models;
using _4Module.Repository;
using Microsoft.EntityFrameworkCore;

namespace _4Module.Services
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;

        public AuthorService(IAuthorRepository authorRepository)
        {
            _authorRepository = authorRepository;
        }

        public async Task<AuthorResponseDTO?> GetByIdAsync(Guid id)
        {
            return await _authorRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAllAsync()
        {
          return await _authorRepository.GetAllAsync();
        }

        public async Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO authorDto)
        {
           return await _authorRepository.CreateAsync(authorDto);
        }

        public async Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO authorDto)
        {
          return await  _authorRepository.UpdateAsync(authorDto);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await (_authorRepository.DeleteAsync(id));
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAuthorsByBookIdAsync(Guid bookId)
        {
            return await _authorRepository.GetAuthorsByBookIdAsync(bookId);
        }
    }
}
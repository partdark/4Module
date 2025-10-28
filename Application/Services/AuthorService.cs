

using Application;
using Application.DTO;
using Domain;
using Domain.Entitties;

namespace Applications.Services
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
            var author = await _authorRepository.GetByIdAsync(id);
            if (author == null) return null;

            return MapToDto(author);
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAllAsync()
        {
            var authors = await _authorRepository.GetAllAsync();
            return authors.Select(MapToDto);
        }

        public async Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO dto)
        {
            var author = new Author
            {
                Name = dto.Name,
                Bio = dto.Bio
            };

            var created = await _authorRepository.CreateAsync(author);
            return MapToDto(created);
        }

        public async Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO dto)
        {
            var author = new Author
            {
                Id = dto.Id,
                Name = dto.Name,
                Bio = dto.Bio
            };

            var updated = await _authorRepository.UpdateAsync(author);
            return updated != null ? MapToDto(updated) : null;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _authorRepository.DeleteAsync(id);
        }

        private AuthorResponseDTO MapToDto(Author author)
        {
            return new AuthorResponseDTO(
                author.Id,
                author.Name,
                author.Bio,
                author.Books.Select(b => new BookDTO(b.Id, b.Title, b.Year)).ToList()
            );
        }


    }
}
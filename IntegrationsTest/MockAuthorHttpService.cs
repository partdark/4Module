using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;

namespace IntegrationTest
{
    public class MockAuthorHttpService : IAuthorHttpService
    {
        public Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            var authors = ids.Select(id => new Author { Id = id, Name = "Mock Author", Bio = "Mock Bio" });
            return Task.FromResult(authors);
        }

        public Task<AuthorResponseDTO?> GetByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<AuthorResponseDTO>> GetAllAsync() => throw new NotImplementedException();
        public Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO dto) => throw new NotImplementedException();
        public Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO dto) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(Guid id) => throw new NotImplementedException();
    }
}


using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;
using System.Net;

namespace IntegrationTest
{
    public class FailingAuthorHttpService : IAuthorHttpService
    {
        private static int _callCount = 0;

        public Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            _callCount++;
            Console.WriteLine($"Call #{_callCount}");

            if (_callCount <= 5)
            {
                throw new HttpRequestException("Service unavailable", null, HttpStatusCode.ServiceUnavailable);
            }

            // После 5 вызовов возвращаем успех
            var authors = ids.Select(id => new Author { Id = id, Name = "Mock Author", Bio = "Mock Bio" });
            return Task.FromResult(authors);
        }

        Task<AuthorResponseDTO> IAuthorHttpService.CreateAsync(CreateAuthorDTO dto)
        {
            throw new NotImplementedException();
        }

        Task<bool> IAuthorHttpService.DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<AuthorResponseDTO>> IAuthorHttpService.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<AuthorResponseDTO?> IAuthorHttpService.GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        Task<AuthorResponseDTO?> IAuthorHttpService.UpdateAsync(UpdateAuthorDTO dto)
        {
            throw new NotImplementedException();
        }

        // Остальные методы...

    }
}


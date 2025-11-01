using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;
using Polly;
using Polly.CircuitBreaker;
using System.Net;

namespace IntegrationTest
{
    public class FailingAuthorHttpService : IAuthorHttpService
    {
        private  int _callCount = 0;

        public Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            _callCount++;
            Console.WriteLine($"Call #{_callCount}");

            if (_callCount <= 5)
            {
                throw new HttpRequestException("Service unavailable", null, HttpStatusCode.ServiceUnavailable);
            }

            
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

       

    }
    public class TestAuthorHttpServiceWithCircuitBreaker : IAuthorHttpService
    {
        private readonly IAsyncPolicy _circuitBreakerPolicy;
        private int _callCount = 0;

        public TestAuthorHttpServiceWithCircuitBreaker()
        {
            _circuitBreakerPolicy = Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(3, TimeSpan.FromSeconds(30));
        }

        public async Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            try
            {
                await _circuitBreakerPolicy.ExecuteAsync(() =>
                {
                    _callCount++;
                    Console.WriteLine($"Call #{_callCount} - throwing exception");
                    throw new HttpRequestException("Service unavailable");
                });

                return new List<Author>();
            }
            catch (BrokenCircuitException)
            {
                Console.WriteLine("Circuit breaker is open - returning fallback");
                return ids.Select(id => new Author
                {
                    Id = id,
                    Name = "Неизвестный автор",
                    Bio = "Автор по умолчанию"
                });
            }
        }

       
        public Task<AuthorResponseDTO?> GetByIdAsync(Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<AuthorResponseDTO>> GetAllAsync() => throw new NotImplementedException();
        public Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO dto) => throw new NotImplementedException();
        public Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO dto) => throw new NotImplementedException();
        public Task<bool> DeleteAsync(Guid id) => throw new NotImplementedException();
    }


}


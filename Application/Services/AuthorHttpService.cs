using Application.DTO;
using Application.Interfaces;
using Domain.Entitties;
using System.Net.Http.Json;
using System.Text.Json;

namespace Application.Services
{


    public class AuthorHttpService : IAuthorHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public AuthorHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public async Task<IEnumerable<Author>> GetByIdsAsync(List<Guid> ids)
        {
            var idsQuery = string.Join("&", ids.Select(x => $"Ids={x}"));

            var response = await _httpClient.GetAsync($"/api/Book/authors/batch?{idsQuery}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Author>>(content, _jsonOptions);
        }
        public async Task<AuthorResponseDTO?> GetByIdAsync(Guid id)
        {
            var response = await _httpClient.GetAsync($"authors/{id}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthorResponseDTO>(content, _jsonOptions);
        }

        public async Task<IEnumerable<AuthorResponseDTO>> GetAllAsync()
        {
            var response = await _httpClient.GetAsync("authors");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<AuthorResponseDTO>>(content, _jsonOptions);
        }

        public async Task<AuthorResponseDTO> CreateAsync(CreateAuthorDTO dto)
        {
            var response = await _httpClient.PostAsJsonAsync("authors", dto, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthorResponseDTO>(content, _jsonOptions);
        }

        public async Task<AuthorResponseDTO?> UpdateAsync(UpdateAuthorDTO dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"authors/{dto.Id}", dto, _jsonOptions);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AuthorResponseDTO?>(content, _jsonOptions);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var response = await _httpClient.DeleteAsync($"authors/{id}");
            return response.IsSuccessStatusCode;
        }




    }
}

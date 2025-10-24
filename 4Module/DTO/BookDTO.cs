namespace _4Module.DTO
{
    public record CreateBookDTO(int Year, List<Guid> AuthorIds, string Title);

    public record UpdateBookDTO(Guid Id, int Year, List<Guid> AuthorIds, string Title);

    public record BookResponseDTO(Guid Id, int Year, List<Guid> AuthorId, string Title, List<AuthorDTO> AuthorName);

    public record CreateAuthorDTO(string Name, string? Bio);

    public record UpdateAuthorDTO(Guid Id, string Name, string? Bio);

    public record AuthorResponseDTO(Guid Id, string Name, string? Bio, List<BookDTO> Books);

    public record AuthorDTO(Guid Id, string Name);

    public record BookDTO(Guid Id, string Title, int Year);
}

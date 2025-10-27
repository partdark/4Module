namespace Application.DTO
{
    public record CreateBookDTO(int Year, List<Guid> AuthorIds, string Title);

    public record UpdateBookDTO(Guid Id, int Year, List<Guid> AuthorIds, string Title);

    public record BookResponseDTO(Guid Id, int Year, List<Guid> AuthorId, string Title, List<AuthorDTO> AuthorName);

    public record CreateAuthorDTO(string Name, string? Bio);

    public record UpdateAuthorDTO(Guid Id, string Name, string? Bio);

    public record AuthorResponseDTO(Guid Id, string Name, string? Bio, List<BookDTO> Books);

    public record AuthorDTO(Guid Id, string Name);

    public record BookDTO(Guid Id, string Title, int Year);

    public record CreateBookWithAuthorDTO(string BookTitle,int Year,string AuthorName,string? AuthorBio = null);


    public record CreateProductReviewDTO ( Guid BookId, string ReviewerName, int Rating, string Comment);

    public record ProductReviewResponseDTO(string Id, Guid BookId, string ReviewerName, int Rating, string Comment, DateTime Date);




    public class AuthorBookCountDTO
    {
        public string AuthorName { get; set; } = string.Empty;
        public long BookCount { get; set; }
    }
}

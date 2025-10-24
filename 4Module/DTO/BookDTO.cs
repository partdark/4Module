namespace _4Module.DTO
{
   

    public record CreateBookDTO(int year,  int AutorId, string title);

    public record UpdateBookDTO(Guid id, int year, int AutorId, string title);

    public record BookResponseDTO(Guid id, int year, int AutorId, string title);
}

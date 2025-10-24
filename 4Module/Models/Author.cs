namespace _4Module.Models
{
    public class Author
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

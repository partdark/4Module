namespace _4Module.Models
{
    public class Book
    {

     
        public Guid Id { get; init; } = Guid.NewGuid();
        public int Year { get; set; }
       
        public string Title { get; set; } = string.Empty;

       
        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;



    }
}



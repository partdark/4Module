namespace _4Module.Models
{
    public class Book
    {

     
        public Guid Id { get; init; } = Guid.NewGuid();
        public int Year { get; set; }
       
        public string Title { get; set; } = string.Empty;

       
        public ICollection<Author> Authors { get; set; } = null!;



    }
}



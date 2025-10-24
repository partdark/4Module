using _4Module.Models;
using Microsoft.EntityFrameworkCore;

namespace _4Module.Data
{
    public class BookContext : DbContext
    {
        public BookContext(DbContextOptions options) : base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Author>(entity =>
            {
                entity.ToTable("Authors");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Bio).HasMaxLength(1000);
                
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.ToTable("Books");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(50);
                entity.Property(e => e.Year).IsRequired();

                entity.HasOne(b => b.Author)
                  .WithMany(a => a.Books)
                  .HasForeignKey(b => b.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);
            });

           
        }
    }
}

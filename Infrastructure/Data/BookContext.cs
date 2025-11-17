

using Domain.Entitties;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Data
{
    public class BookContext : IdentityDbContext<IdentityUser>
    {
        public BookContext(DbContextOptions<BookContext> options) : base(options)
        {
            
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

                entity.HasMany(b => b.Authors)
                  .WithMany(a => a.Books)
                  .UsingEntity(j => j.ToTable("BookAuthors"));
            });

           
        }
    }
}

using BookStore.Base.Implementations.BaseResources.Inner;
using BookStore.BookService.BL.ResourceEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BookService.Data.BaseDatabase;

public class BaseDbContext : BaseBookStoreDbContext
{
    public BaseDbContext(DbContextOptions<BaseDbContext> options)
        : base(options)
    {
    }

    public DbSet<BL.ResourceEntities.Book> Books { get; set; }
    public DbSet<BookCategory> BookCategories { get; set; }
    public DbSet<Author> Authors { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        SchemeSettings(modelBuilder);

        modelBuilder.AddInboxStateEntity();
        modelBuilder.AddOutboxMessageEntity();
        modelBuilder.AddOutboxStateEntity();
    }

    // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    // {
    //     optionsBuilder.EnableServiceProviderCaching(false);
    //     optionsBuilder.LogTo(Console.WriteLine);
    //     // if (!optionsBuilder.IsConfigured)
    //     // {
    //     //     optionsBuilder.UseNpgsql(_databaseConfig.ConnectionString);
    //     // }
    // }
    private static void SchemeSettings(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<BL.ResourceEntities.Book>()
            .HasKey(book => book.BookId);

        modelBuilder
            .Entity<Author>()
            .HasKey(author => author.AuthorId);

        modelBuilder
            .Entity<BookCategory>()
            .HasKey(bookCategory => bookCategory.CategoryId);

        modelBuilder
            .Entity<BL.ResourceEntities.Book>()
            .HasMany(book => book.Authors)
            .WithMany(author => author.Books);

        modelBuilder
            .Entity<BL.ResourceEntities.Book>()
            .HasMany(book => book.Categories)
            .WithMany(category => category.Books);

        //modelBuilder.SeedData();
    }
}
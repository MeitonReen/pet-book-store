using BookStore.Base.Implementations.BaseResources.Inner;
using BookStore.UserService.BL.ResourceEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.Data.BaseDatabase
{
    public class BaseDbContext : BaseBookStoreDbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookRating> BookRatings { get; set; }
        public DbSet<BookReview> BookReviews { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<BL.ResourceEntities.Profile> Profiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SchemeSettings(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private void SchemeSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BL.ResourceEntities.Profile>().HasKey(entity => entity.UserId);
            modelBuilder.Entity<Book>().HasKey(entity => entity.BookId);
            modelBuilder.Entity<BookReview>().HasKey(entity => entity.ReviewId);
            modelBuilder.Entity<BookRating>().HasKey(entity => entity.RatingId);

            modelBuilder.Entity<BL.ResourceEntities.Profile>()
                .HasMany(entity => entity.BookRatings)
                .WithOne(entity => entity.Profile);
            modelBuilder.Entity<BL.ResourceEntities.Profile>()
                .HasMany(entity => entity.BookReviews)
                .WithOne(entity => entity.Profile);

            modelBuilder.Entity<Book>()
                .HasMany(entity => entity.BookRatings)
                .WithOne(entity => entity.Book)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Book>()
                .HasMany(entity => entity.BookReviews)
                .WithOne(entity => entity.Book)
                .OnDelete(DeleteBehavior.SetNull);

            //modelBuilder.SeedData();
        }
    }
}
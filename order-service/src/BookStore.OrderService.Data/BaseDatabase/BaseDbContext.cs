using BookStore.Base.Implementations.BaseResources.Inner;
using BookStore.OrderService.BL.ResourceEntities;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.Data.BaseDatabase
{
    public class BaseDbContext : BaseBookStoreDbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options)
            : base(options)
        {
        }

        public DbSet<Profile> Profiles { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<BookInCart> BooksInCarts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<BookInOrder> BooksInOrders { get; set; }

        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseNpgsql(_databaseConfig.ConnectionString);
        //     }
        // }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            SchemeSettings(modelBuilder);

            modelBuilder.AddInboxStateEntity();
            modelBuilder.AddOutboxMessageEntity();
            modelBuilder.AddOutboxStateEntity();
        }

        private static void SchemeSettings(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Profile>().HasKey(entity => entity.UserId);
            modelBuilder.Entity<Book>().HasKey(entity => entity.BookId);
            modelBuilder.Entity<Cart>().HasKey(entity => entity.CartId);
            modelBuilder.Entity<Order>().HasKey(entity => entity.OrderId);
            modelBuilder.Entity<BookInCart>().HasKey(entity => entity.BookInCartId);
            modelBuilder.Entity<BookInOrder>().HasKey(entity => entity.BookInOrderId);

            modelBuilder.Entity<Profile>()
                .HasMany(entity => entity.Orders)
                .WithOne(entity => entity.Profile);
            modelBuilder.Entity<Profile>()
                .HasMany(entity => entity.Carts)
                .WithOne(entity => entity.Profile);

            modelBuilder.Entity<Book>()
                .HasMany(entity => entity.Carts)
                .WithOne(entity => entity.Book);
            modelBuilder.Entity<Book>()
                .HasMany(entity => entity.Orders)
                .WithOne(entity => entity.Book);

            //modelBuilder.SeedData();
        }
    }
}
using System;
using BookStore.OrderService.BL.ResourceEntities;

namespace BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
//With reference/s => new object, for ensure idempotency init

public static class InitDataCreator
{
    public static class Profiles
    {
        public static Profile TestUser => new()
        {
            UserId = new Guid("08247ee1-a292-4029-80e1-bb50ca4a4743")
        };
    }

    public static class Carts
    {
        public static Cart TestUserCart => new()
        {
            CartId = new Guid("053c350d-7659-428e-920f-70cfcc96daaf"),
            CreationDateTime = DateTime.Now
            // Profile = Profiles.TestUser,
            // Books = new List<BookInCart> {BooksInTestUserCart.TheGambler}
        };
    }

    public static class BooksInTestUserCart
    {
        public static BookInCart TheGambler => new()
        {
            BookInCartId = new Guid("ec1d2a1d-0eea-473c-a96c-4ebe4f40d3a1"),
            // Book = Books.TheGambler,
            Count = 2
        };
    }

    public static class Books
    {
        public static Book TheGambler => new()
        {
            BookId = new Guid("FD50B9BA-0C9C-442C-9A0C-238A39BC2A9B"),
            Deleted = false,
            Name = "The gambler",
            Price = 800,
            PublicationDate = DateOnly.FromDateTime(DateTime.Now)
        };

        public static Book CrimeAndPunishment => new()
        {
            BookId = new Guid("E08A8A64-A289-4773-AAB1-5C9355A74228"),
            Deleted = false,
            Name = "Grime and punishment",
            Price = 900,
            PublicationDate = DateOnly.FromDateTime(DateTime.Now)
        };

        public static Book EugeneOnegin => new()
        {
            BookId = new Guid("5EB79A79-687F-40F5-98B1-CD9F2DDFDE54"),
            Deleted = false,
            Name = "Eugene Onegin",
            Price = 790,
            PublicationDate = DateOnly.FromDateTime(DateTime.Now)
        };
    }

    public static class TestUserOrders
    {
        public static Order OrderOne => new()
        {
            OrderId = new Guid("90659880-39d5-45d0-9341-2190a41776f2"),
            Amount = 2390,
            CreationDateTime = DateTime.Now,
            // Profile = Profiles.TestUser,
            // Books = new[]
            // {
            //     BookInOneOrderTestUser.TheGambler,
            //     BookInOneOrderTestUser.EugeneOnegin
            // }
        };

        public static Order OrderTwo => new()
        {
            OrderId = new Guid("71e7324f-3f0a-4240-9b62-ea522ea6094e"),
            Amount = 2600,
            CreationDateTime = DateTime.Now,
            // Profile = Profiles.TestUser,
            // Books = new[]
            // {
            //     BookInTwoOrderTestUser.TheGambler,
            //     BookInTwoOrderTestUser.CrimeAndPunishment
            // }
        };
    }

    public static class BookInTestUserOneOrder
    {
        public static BookInOrder TheGambler => new()
        {
            BookInOrderId = new Guid("a4d5f2fc-8fcb-4244-8fe4-1ffec82d54a7"),
            // Book = Books.TheGambler,
            Count = 2
        };

        public static BookInOrder EugeneOnegin => new()
        {
            BookInOrderId = new Guid("33a1d8e3-60e8-49d1-8786-249d31dede88"),
            // Book = Books.EugeneOnegin,
            Count = 1
        };
    }

    public static class BookInTestUserTwoOrder
    {
        public static BookInOrder TheGambler => new()
        {
            BookInOrderId = new Guid("aba3a4ac-5cd6-4dda-8693-d52143b82a90"),
            // Book = Books.TheGambler,
            Count = 1
        };

        public static BookInOrder CrimeAndPunishment => new()
        {
            BookInOrderId = new Guid("95b4ef4e-a974-44d1-90c1-8b5a3e56028e"),
            // Book = Books.CrimeAndPunishment,
            Count = 2
        };
    }
}
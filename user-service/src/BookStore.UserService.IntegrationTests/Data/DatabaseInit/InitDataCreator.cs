using System;
using BookStore.UserService.BL.ResourceEntities;

namespace BookStore.UserService.IntegrationTests.Data.DatabaseInit;

public static class InitDataCreator
{
    public static class Profiles
    {
        public static Profile TestUser => new()
        {
            UserId = new Guid("08247ee1-a292-4029-80e1-bb50ca4a4743"),
            UserName = "TestUser",
            Patronymic = "TestUserPatronymic",
            FirstName = "TestUserFirstName",
            LastName = "TestUserLastName"
        };
    }

    public static class Books
    {
        public static Book TheGambler => new()
        {
            BookId = new Guid("FD50B9BA-0C9C-442C-9A0C-238A39BC2A9B")
        };

        public static Book CrimeAndPunishment => new()
        {
            BookId = new Guid("E08A8A64-A289-4773-AAB1-5C9355A74228")
        };

        public static Book EugeneOnegin => new()
        {
            BookId = new Guid("5EB79A79-687F-40F5-98B1-CD9F2DDFDE54")
        };
    }

    public static class BookRatings
    {
        public static BookRating TheGamblerTestUserRating => new()
        {
            RatingId = new Guid("cd662bc0-2bd0-4028-a664-54855c4e3ef8"),
            // Profile = Profiles.TestUser,
            // Book = Books.TheGambler,
            Rating = 7,
            DateTimeSet = DateTime.Now
        };

        public static BookRating CrimeAndPunishmentTestUserRating => new()
        {
            RatingId = new Guid("66066df3-a6f6-45ad-83c2-a2fa54662921"),
            // Profile = Profiles.TestUser,
            // Book = Books.CrimeAndPunishment,
            Rating = 8,
            DateTimeSet = DateTime.Now
        };

        public static BookRating EugeneOneginTestUserRating => new()
        {
            RatingId = new Guid("a2a81be7-951d-4498-b507-613c72f2e682"),
            // Profile = Profiles.TestUser,
            // Book = Books.EugeneOnegin,
            Rating = 8,
            DateTimeSet = DateTime.Now
        };
    }

    public static class BookReviews
    {
        public static BookReview TheGamblerTestUserReview => new()
        {
            ReviewId = new Guid("986aa9d8-51af-4c3f-95a5-546d11890b24"),
            // Profile = Profiles.TestUser,
            // Book = Books.TheGambler,
            Review = "Test review",
            DateTimeSet = DateTime.Now
        };

        public static BookReview CrimeAndPunishmentTestUserReview => new()
        {
            ReviewId = new Guid("69386484-b8da-4200-8e1d-9279270d718b"),
            // Profile = Profiles.TestUser,
            // Book = Books.CrimeAndPunishment,
            Review = "Test review",
            DateTimeSet = DateTime.Now
        };

        public static BookReview EugeneOneginTestUserReview => new()
        {
            ReviewId = new Guid("2069d310-2085-476c-a7b3-b34a04605432"),
            // Profile = Profiles.TestUser,
            // Book = Books.EugeneOnegin,
            Review = "Test review",
            DateTimeSet = DateTime.Now
        };
    }
}
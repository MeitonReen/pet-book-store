using System;
using System.Threading.Tasks;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.NonAttachedExtensions;
using BookStore.UserService.Data.BaseDatabase;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.IntegrationTests.Data.DatabaseInit;

public class DatabaseInitRuntime : IDatabaseInit
{
    private readonly BaseDbContext _dbContext;

    public DatabaseInitRuntime(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(Func<DbContext, Task>? dbInitSettings = default)
    {
        try
        {
            await _dbContext.Database.MigrateAsync();

            await SeedData();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SeedData(Action<DbContext>? dbInitSettings = default)
    {
        if (dbInitSettings != default)
        {
            dbInitSettings(_dbContext);
            await _dbContext.SaveChangesAsync();

            return;
        }

        var eugeneOneginTestUserRating = await _dbContext.BookRatings
            .AddIfNotContain(InitDataCreator.BookRatings.EugeneOneginTestUserRating);
        var theGamblerTestUserRating = await _dbContext.BookRatings
            .AddIfNotContain(InitDataCreator.BookRatings.TheGamblerTestUserRating);
        var crimeAndPunishmentTestUserRating = await _dbContext.BookRatings
            .AddIfNotContain(InitDataCreator.BookRatings.CrimeAndPunishmentTestUserRating);

        var eugeneOneginTestUserReview = await _dbContext.BookReviews
            .AddIfNotContain(InitDataCreator.BookReviews.EugeneOneginTestUserReview);
        var theGamblerTestUserReview = await _dbContext.BookReviews
            .AddIfNotContain(InitDataCreator.BookReviews.TheGamblerTestUserReview);
        var crimeAndPunishmentTestUserReview = await _dbContext.BookReviews
            .AddIfNotContain(InitDataCreator.BookReviews.CrimeAndPunishmentTestUserReview);

        await _dbContext.Books.AddIfNotContain(
            InitDataCreator.Books.EugeneOnegin
                .ShareTo(eugeneOnegin =>
                {
                    eugeneOneginTestUserRating.Book = eugeneOnegin;
                    eugeneOneginTestUserReview.Book = eugeneOnegin;
                }),
            InitDataCreator.Books.TheGambler
                .ShareTo(theGambler =>
                {
                    theGamblerTestUserRating.Book = theGambler;
                    theGamblerTestUserReview.Book = theGambler;
                }),
            InitDataCreator.Books.CrimeAndPunishment
                .ShareTo(crimeAndPunishment =>
                {
                    crimeAndPunishmentTestUserRating.Book = crimeAndPunishment;
                    crimeAndPunishmentTestUserReview.Book = crimeAndPunishment;
                })
        );

        var d = InitDataCreator.Profiles.TestUser
            .ShareTo(testUser =>
            {
                eugeneOneginTestUserRating.Profile = testUser;
                theGamblerTestUserRating.Profile = testUser;
                crimeAndPunishmentTestUserRating.Profile = testUser;

                eugeneOneginTestUserReview.Profile = testUser;
                theGamblerTestUserReview.Profile = testUser;
                crimeAndPunishmentTestUserReview.Profile = testUser;
            });

        await _dbContext.Profiles.AddIfNotContain(InitDataCreator.Profiles.TestUser
            .ShareTo(testUser =>
            {
                eugeneOneginTestUserRating.Profile = testUser;
                theGamblerTestUserRating.Profile = testUser;
                crimeAndPunishmentTestUserRating.Profile = testUser;

                eugeneOneginTestUserReview.Profile = testUser;
                theGamblerTestUserReview.Profile = testUser;
                crimeAndPunishmentTestUserReview.Profile = testUser;
            })
        );

        await _dbContext.SaveChangesAsync();
    }
}
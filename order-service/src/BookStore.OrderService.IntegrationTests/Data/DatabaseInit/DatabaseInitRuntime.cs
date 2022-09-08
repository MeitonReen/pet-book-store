using System;
using System.Threading.Tasks;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.NonAttachedExtensions;
using BookStore.OrderService.Data.BaseDatabase;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.IntegrationTests.Data.DatabaseInit;

public class DatabaseInitRuntime : IDatabaseInit
{
    private readonly BaseDbContext _dbContext;

    public DatabaseInitRuntime(BaseDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(Func<DbContext, Task>? dbInitSettings = null)
    {
        try
        {
            await _dbContext.Database.MigrateAsync();

            await SeedData(dbInitSettings);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task SeedData(Func<DbContext, Task>? dbInitSettings = default)
    {
        if (dbInitSettings != default)
        {
            await dbInitSettings(_dbContext);
            await _dbContext.SaveChangesAsync();

            return;
        }

        #region TestUserCart

        var testUserCart = await _dbContext.Carts.AddIfNotContain(InitDataCreator.Carts.TestUserCart);

        var theGamblerBookInTestUserCart = (await _dbContext.BooksInCarts
                .AddIfNotContain(InitDataCreator.BooksInTestUserCart.TheGambler))
            .ShareTo(bookInTestUserCart => testUserCart.Books.Add(bookInTestUserCart));

        #endregion

        #region OrderOne

        var testUserOrderOne = await _dbContext.Orders.AddIfNotContain(
            InitDataCreator.TestUserOrders.OrderOne);

        var theGamblerBookInTestUserOneOrder = (await _dbContext.BooksInOrders
                .AddIfNotContain(InitDataCreator.BookInTestUserOneOrder.TheGambler))
            .ShareTo(bookInOneOrderTestUser => testUserOrderOne.Books.Add(bookInOneOrderTestUser));

        var eugeneOneginBookInTestUserOneOrder = (await _dbContext.BooksInOrders
                .AddIfNotContain(InitDataCreator.BookInTestUserOneOrder.EugeneOnegin))
            .ShareTo(bookInOneOrderTestUser => testUserOrderOne.Books.Add(bookInOneOrderTestUser));

        #endregion

        #region OrderTwo

        var testUserOrderTwo = await _dbContext.Orders.AddIfNotContain(
            InitDataCreator.TestUserOrders.OrderTwo);

        var theGamblerBookInTestUserTwoOrder = (await _dbContext.BooksInOrders
                .AddIfNotContain(InitDataCreator.BookInTestUserTwoOrder.TheGambler))
            .ShareTo(bookInOneOrderTestUser => testUserOrderTwo.Books.Add(bookInOneOrderTestUser));

        var crimeAndPunishmentBookInTestUserTwoOrder = (await _dbContext.BooksInOrders
                .AddIfNotContain(InitDataCreator.BookInTestUserTwoOrder.CrimeAndPunishment))
            .ShareTo(bookInOneOrderTestUser => testUserOrderTwo.Books.Add(bookInOneOrderTestUser));

        #endregion

        #region Books

        await _dbContext.Books.AddIfNotContain(
            InitDataCreator.Books.EugeneOnegin.ShareTo(eugeneOnegin =>
                eugeneOneginBookInTestUserOneOrder.Book = eugeneOnegin),
            InitDataCreator.Books.TheGambler.ShareTo(theGambler =>
                {
                    theGamblerBookInTestUserCart.Book = theGambler;
                    theGamblerBookInTestUserOneOrder.Book = theGambler;
                    theGamblerBookInTestUserTwoOrder.Book = theGambler;
                }
            ),
            InitDataCreator.Books.CrimeAndPunishment.ShareTo(crimeAndPunishment =>
                crimeAndPunishmentBookInTestUserTwoOrder.Book = crimeAndPunishment)
        );

        #endregion

        #region profile

        await _dbContext.Profiles.AddIfNotContain(InitDataCreator.Profiles.TestUser
            .ShareTo(testUser =>
            {
                testUserCart.Profile = testUser;
                testUserOrderOne.Profile = testUser;
                testUserOrderTwo.Profile = testUser;
            }));

        #endregion

        await _dbContext.SaveChangesAsync();
    }
}
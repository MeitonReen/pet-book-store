using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.NonAttachedExtensions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BookService.Data.BaseDatabase.DatabaseInit;

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
            if (await _dbContext.Database.CanConnectAsync())
            {
                await _dbContext.Database.MigrateAsync();
                return;
            }

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

        var eugeneOnegin = await _dbContext.Books.AddIfNotContain(InitDataCreator.Books.EugeneOnegin);
        var theGambler = await _dbContext.Books.AddIfNotContain(InitDataCreator.Books.TheGambler);
        var crimeAndPunishment = await _dbContext.Books.AddIfNotContain(InitDataCreator.Books.CrimeAndPunishment);

        await _dbContext.Authors.AddIfNotContain(
            InitDataCreator.Authors.Chekhov,
            InitDataCreator.Authors.Dostoevskiy
                .ShareTo(dostoevskiy => theGambler.Authors.Add(dostoevskiy))
                .ShareTo(dostoevskiy => crimeAndPunishment.Authors.Add(dostoevskiy)),
            InitDataCreator.Authors.Pushkin.ShareTo(pushkin => eugeneOnegin.Authors.Add(pushkin)));

        await _dbContext.BookCategories.AddIfNotContain(
            InitDataCreator.BookCategories.Classic
                .ShareTo(classic => eugeneOnegin.Categories.Add(classic))
                .ShareTo(classic => theGambler.Categories.Add(classic))
                .ShareTo(classic => crimeAndPunishment.Categories.Add(classic)),
            InitDataCreator.BookCategories.Poetry);

        await _dbContext.SaveChangesAsync();
    }
}
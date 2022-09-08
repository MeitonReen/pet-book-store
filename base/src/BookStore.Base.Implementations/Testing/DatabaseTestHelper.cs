using BookStore.Base.Implementations.DatabaseInit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.Testing;

public static class DatabaseTestHelper
{
    public static async Task Initialize(
        IEnumerable<IDatabaseInit> databaseInits,
        DbContext dbContext)
        => await databaseInits
            .ToAsyncEnumerable()
            .ForEachAwaitAsync(async initer =>
            {
                await initer.SeedAsync();
                dbContext.ChangeTracker.Clear();
            });

    public static async Task Initialize(
        IDatabaseInit databaseInit,
        DbContext dbContext,
        Func<DbContext, Task>? dbInitSettings = default)
    {
        await databaseInit.SeedAsync(dbInitSettings);
        dbContext.ChangeTracker.Clear();
    }
}
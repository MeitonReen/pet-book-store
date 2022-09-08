using BookStore.Base.Implementations.DatabaseInit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.Data.SagasDatabase.DatabaseInit;

public class DatabaseInitRuntime : IDatabaseInit
{
    private readonly SagasDbContext _dbContext;

    public DatabaseInitRuntime(SagasDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task SeedAsync(Func<DbContext, Task>? dbInitSettings = default)
    {
        try
        {
            await _dbContext.Database.MigrateAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private Task SeedData(Action<DbContext>? dbInitSettings = default)
    {
        return Task.CompletedTask;
    }
}
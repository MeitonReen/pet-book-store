using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookStore.BookService.Data.BaseDatabase;

public class BaseDbContextDesignTimeFactory : IDesignTimeDbContextFactory<BaseDbContext>
{
    private const string ConnectionStringEnvironmentVariable = "BASE_DB_CONNECTION_STRING_DESIGN_TIME";

    public BaseDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment
            .GetEnvironmentVariable(ConnectionStringEnvironmentVariable);

        if (connectionString == default)
            throw new ArgumentNullException(
                nameof(connectionString),
                $"Design-time connection string is not set in {ConnectionStringEnvironmentVariable}");

        var optionsBuilder = new DbContextOptionsBuilder<BaseDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new BaseDbContext(optionsBuilder.Options);
    }
}
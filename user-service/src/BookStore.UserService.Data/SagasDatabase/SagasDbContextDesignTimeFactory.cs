using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BookStore.UserService.Data.SagasDatabase;

public class SagasDbContextDesignTimeFactory : IDesignTimeDbContextFactory<SagasDbContext>
{
    private const string ConnectionStringEnvironmentVariable = "SAGAS_DB_CONNECTION_STRING_DESIGN_TIME";

    public SagasDbContext CreateDbContext(string[] args)
    {
        var connectionString = Environment
            .GetEnvironmentVariable(ConnectionStringEnvironmentVariable);

        if (connectionString == default)
            throw new ArgumentNullException(
                nameof(connectionString),
                $"Design-time connection string is not set in {ConnectionStringEnvironmentVariable}");

        var optionsBuilder = new DbContextOptionsBuilder<SagasDbContext>();
        optionsBuilder.UseNpgsql(connectionString);

        return new SagasDbContext(optionsBuilder.Options);
    }
}
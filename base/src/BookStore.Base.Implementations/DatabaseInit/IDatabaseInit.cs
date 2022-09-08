using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.DatabaseInit;

public interface IDatabaseInit
{
    Task SeedAsync(Func<DbContext, Task>? dbInitSettings = default);
}
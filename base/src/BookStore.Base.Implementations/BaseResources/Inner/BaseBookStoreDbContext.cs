using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner;

public class BaseBookStoreDbContext : DbContext
{
    public BaseBookStoreDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
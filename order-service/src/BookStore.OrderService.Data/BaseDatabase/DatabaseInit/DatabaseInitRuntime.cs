using BookStore.Base.Implementations.DatabaseInit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.Data.BaseDatabase.DatabaseInit
{
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
            /*IfNotContainThenAdd(entitiesDict.Values.ToArray());
            
            await _dbContext.SaveChangesAsync();*/
        }
        /*private void IfNotContainThenAdd(entityType[] entities)
        {
            Array.ForEach(entities, entity =>
            {
                if (!_dbContext.Entities.Any(innerIntity => innerIntity.LastName == innerIntity.LastName))
                {
                    _dbContext.Entities.Add(entity);
                }
            });
        }*/
    }
}
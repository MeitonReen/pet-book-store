using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.NonAttachedExtensions;

public static class DbSetExtensions
{
    public static Task AddIfNotContain<TEntity>(this DbSet<TEntity> store, params TEntity[] entitiesToAdd)
        where TEntity : class
        => entitiesToAdd
            .ToAsyncEnumerable()
            .ForEachAwaitAsync(async el =>
            {
                if (!await store.ContainsAsync(el)) store.Add(el);
                else store.Attach(el);
            });

    // public static TEntity AddIfNotContain<TEntity>(this DbSet<TEntity> store, TEntity entityToAdd)
    //     where TEntity : class
    // {
    //     if (!store.Contains(entityToAdd)) store.Add(entityToAdd);
    //     return entityToAdd;
    // }

    public static async Task<TEntity> AddIfNotContain<TEntity>(this DbSet<TEntity> store, TEntity entityToAdd)
        where TEntity : class
    {
        if (!await store.ContainsAsync(entityToAdd)) store.Add(entityToAdd);
        else store.Attach(entityToAdd);

        return entityToAdd;
    }

    public static async Task AddIfNotExistsAsync<T>(this DbSet<T> dbSet, T entity,
        Expression<Func<T, bool>>? predicate = default) where T : class, new()
    {
        var exists = (predicate != default)
            ? await dbSet.AnyAsync(predicate)
            : await dbSet.AnyAsync(entityIn => entityIn == entity);

        if (!exists)
        {
            dbSet.Add(entity);
        }
    }
}
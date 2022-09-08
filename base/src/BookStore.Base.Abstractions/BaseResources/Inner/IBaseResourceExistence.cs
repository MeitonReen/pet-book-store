using System.Linq.Expressions;

namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IBaseResourceExistence<TResourceEntity>
        where TResourceEntity : class
    {
        IQueryable<TResourceEntity> Query { get; }
        Task<bool> ReadAsync();

        IBaseResourceExistence<TResourceEntity> ReadSettings(
            Expression<Func<TResourceEntity, bool>> resourceEntityPredicate);

        IBaseResourceExistence<TResourceEntity> ReadSettings(
            TResourceEntity readSettings);

        IBaseResourceExistence<TResourceEntity> ReadSettings(
            Action<TResourceEntity> readSettings);

        IBaseResourceExistence<TResourceEntity> ResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                resourceCollectionSettings);

        IBaseResourceExistence<TResourceEntity> AddResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                resourceCollectionSettings);
    }
}
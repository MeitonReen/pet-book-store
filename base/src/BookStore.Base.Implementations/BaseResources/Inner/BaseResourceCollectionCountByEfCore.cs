using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner
{
    public class BaseResourceCollectionCountByEfCore<TResourceEntity> :
        IBaseResourceCollectionCount<TResourceEntity>
        where TResourceEntity : class, new()
    {
        private readonly IBaseResourceCollection<TResourceEntity> _baseResourceCollection;

        private readonly Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _defaultResourceCollectionReadSettings = sets => sets.AsNoTracking();

        public BaseResourceCollectionCountByEfCore(
            IBaseResourceCollection<TResourceEntity> baseResourceCollection)
        {
            _baseResourceCollection = baseResourceCollection;
        }

        public IQueryable<TResourceEntity> Query => _baseResourceCollection.Query;

        public Task<int> ReadAsync() => _baseResourceCollection
            .Query
            .CountAsync();

        public Task<long> ReadLongAsync() => _baseResourceCollection
            .Query
            .LongCountAsync();

        public IBaseResourceCollectionCount<TResourceEntity> ResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> resourceCollectionSettings)
        {
            _baseResourceCollection.ReadSettings(sets =>
                resourceCollectionSettings(
                    _defaultResourceCollectionReadSettings(sets)));
            return this;
        }

        public IBaseResourceCollectionCount<TResourceEntity> AddResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> resourceCollectionSettings)
        {
            _baseResourceCollection.AddReadSettings(sets =>
                resourceCollectionSettings(
                    _defaultResourceCollectionReadSettings(sets)));
            return this;
        }
    }
}
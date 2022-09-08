using System.Linq.Expressions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner
{
    public class BaseResourceExistenceByEfCore<TResourceEntity> :
        IBaseResourceExistence<TResourceEntity>
        where TResourceEntity : class, new()
    {
        private readonly IBaseResourceCollection<TResourceEntity> _baseResourceCollection;

        private readonly Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _defaultResourceCollectionReadSettings = sets => sets.AsNoTracking();

        private TResourceEntity? _resourceEntity;

        private Expression<Func<TResourceEntity, bool>>? _resourceEntityPredicate;

        public BaseResourceExistenceByEfCore(
            IBaseResourceCollection<TResourceEntity> baseResourceCollection)
        {
            _baseResourceCollection = baseResourceCollection;
        }

        public Task<bool> ReadAsync()
            => _resourceEntityPredicate != default
                ? _baseResourceCollection.Query.AnyAsync(_resourceEntityPredicate)
                : _baseResourceCollection.Query.ContainsAsync(_resourceEntity);

        public IBaseResourceExistence<TResourceEntity>
            ReadSettings(Expression<Func<TResourceEntity, bool>> resourceEntityPredicate)
        {
            _resourceEntityPredicate = resourceEntityPredicate;
            return this;
        }

        public IBaseResourceExistence<TResourceEntity>
            ReadSettings(TResourceEntity resourceEntity)
        {
            _resourceEntity = resourceEntity;
            return this;
        }

        public IBaseResourceExistence<TResourceEntity>
            ReadSettings(Action<TResourceEntity> readSettings)
        {
            var targetResource = new TResourceEntity();
            readSettings(targetResource);
            _resourceEntity = targetResource;

            return this;
        }

        public IBaseResourceExistence<TResourceEntity>
            ResourceCollectionSettings(
                Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                    resourceCollectionSettings)
        {
            _baseResourceCollection.ReadSettings(sets =>
                resourceCollectionSettings(
                    _defaultResourceCollectionReadSettings(sets)));

            return this;
        }

        public IBaseResourceExistence<TResourceEntity>
            AddResourceCollectionSettings(
                Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                    resourceCollectionSettings)
        {
            _baseResourceCollection.AddReadSettings(sets =>
                resourceCollectionSettings(
                    _defaultResourceCollectionReadSettings(sets)));
            return this;
        }

        public IQueryable<TResourceEntity> Query => _baseResourceCollection.Query;
    }
}
using System.Linq.Expressions;
using System.Reflection;
using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner
{
    public class BaseResourceCollectionByEfCore<TResourceEntity> :
        IBaseResourceCollection<TResourceEntity>
        where TResourceEntity : class, new()
    {
        private readonly DbSet<TResourceEntity> _baseResourceEntityCollection;
        private readonly BaseBookStoreDbContext _bookStoreDbContext;

        private readonly Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _targetResourceEntityCollectionSelector;

        private Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _readSettings;

        public BaseResourceCollectionByEfCore(BaseBookStoreDbContext bookStoreDbContext,
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>?
                targetResourceCollectionSelector = default)
        {
            _bookStoreDbContext = bookStoreDbContext;
            _baseResourceEntityCollection = _bookStoreDbContext.Set<TResourceEntity>();

            _targetResourceEntityCollectionSelector = targetResourceCollectionSelector
                                                      ?? (_ => _baseResourceEntityCollection);
            _readSettings = readSettings => readSettings;
        }

        public IQueryable<TResourceEntity> Query => _readSettings(
            _targetResourceEntityCollectionSelector(_baseResourceEntityCollection));

        public IEnumerable<TResourceEntity> Create(
            IEnumerable<TResourceEntity> targetResourceEntityCollection)
        {
            var targetResourceEntityArray = targetResourceEntityCollection.ToArray();
            _baseResourceEntityCollection.AddRange(targetResourceEntityArray);

            return targetResourceEntityArray;
        }

        public IEnumerable<TResourceEntity> Read() =>
            _readSettings(
                _targetResourceEntityCollectionSelector(_baseResourceEntityCollection));

        public async Task<IEnumerable<TResourceEntity>> ReadAsync() =>
            await _readSettings(
                    _targetResourceEntityCollectionSelector(_baseResourceEntityCollection))
                .ToArrayAsync();

        public IBaseResourceCollection<TResourceEntity> ReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings)
        {
            _readSettings = readSettings;
            return this;
        }

        public IBaseResourceCollection<TResourceEntity> AddReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings)
        {
            _readSettings = readQuery => readSettings(_readSettings(readQuery));
            return this;
        }

        public IReadableDataCollectionFromResourceCollection<TResultDataItem>
            ReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>,
                    IQueryable<TResultDataItem>> readSettings) where TResultDataItem : class
            => new DataCollectionFromResourceCollectionByEfCore<TResultDataItem>
            (readSettings(
                _readSettings(
                    _targetResourceEntityCollectionSelector(_baseResourceEntityCollection))));

        public IReadableDataCollectionFromResourceCollection<TResultDataItem>
            AddReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>,
                    IQueryable<TResultDataItem>> readSettings) where TResultDataItem : class
            => new DataCollectionFromResourceCollectionByEfCore<TResultDataItem>
            (readSettings(
                _readSettings(
                    _targetResourceEntityCollectionSelector(_baseResourceEntityCollection))));


        public IEnumerable<TResourceEntity> Update(
            IEnumerable<TResourceEntity> targetResourceEntityCollection)
        {
            var targetResourceEntityArray = targetResourceEntityCollection.ToArray();
            _baseResourceEntityCollection.UpdateRange(targetResourceEntityArray);

            return targetResourceEntityArray;
        }

        public IEnumerable<TResourceEntity> Delete(
            IEnumerable<TResourceEntity> targetResourceEntityCollection)
        {
            var targetResourceEntityArray = targetResourceEntityCollection.ToArray();
            _baseResourceEntityCollection.RemoveRange(targetResourceEntityArray);

            return targetResourceEntityArray;
        }

        public IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            Array.ForEach(targetResourceEntityCollection.ToArray(), el =>
                CreateReferenceInner(el, refToCollectionInTargetResourceEntity,
                    refResourceEntity));

            return this;
        }

        public IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TRefResourceEntity();
            refResourceEntitySettings(targetResourceEntity);

            CreateReferenceInner(
                targetResourceEntityCollection,
                refToCollectionInTargetResourceEntity,
                targetResourceEntity);

            return this;
        }

        public IBaseResourceCollection<TResourceEntity> CreateReferencesInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class, new()
        {
            Array.ForEach(targetResourceEntityCollection.ToArray(), el =>
                CreateReferencesInner(el,
                    refToCollectionInTargetResourceEntity,
                    refResourceEntityCollection));

            return this;
        }

        public IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TRefResourceEntity();
            refResourceEntitySettings(targetResourceEntity);

            CreateReferenceInner(
                targetResourceEntityCollection,
                refToResourceInTargetResourceEntity,
                targetResourceEntity);

            return this;
        }

        private void CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
            => Array.ForEach(targetResourceEntityCollection.ToArray(), el =>
                CreateReferenceInner(el, refToResourceInTargetResourceEntity,
                    refResourceEntity));

        private void CreateReferenceInner<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToResourceInTargetResourceEntityCollection,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            var collectionInTargetResourceEntity =
                refToResourceInTargetResourceEntityCollection(
                    targetResourceEntity);

            if (_bookStoreDbContext.Entry(targetResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(targetResourceEntity);
            }

            if (_bookStoreDbContext.Entry(refResourceEntity).State
                == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(refResourceEntity);
            }

            collectionInTargetResourceEntity.Add(
                refResourceEntity);
        }

        private void CreateReferencesInner<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class, new()
        {
            var collectionInTargetResourceEntity = refToCollectionInTargetResourceEntity(
                targetResourceEntity);

            if (_bookStoreDbContext.Entry(targetResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(targetResourceEntity);
            }

            refResourceEntityCollection = refResourceEntityCollection
                .Select(resourceEntity =>
                {
                    if (_bookStoreDbContext.Entry(resourceEntity).State == EntityState.Detached)
                    {
                        _bookStoreDbContext.Attach(resourceEntity);
                    }

                    return resourceEntity;
                });

            Array.ForEach(refResourceEntityCollection.ToArray(), el =>
                collectionInTargetResourceEntity.Add(el));
        }

        private void CreateReferenceInner<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            if (refToResourceInTargetResourceEntity == default)
                throw new ArgumentNullException(nameof(refToResourceInTargetResourceEntity));

            if (refToResourceInTargetResourceEntity.Body
                is not MemberExpression memberExpression)
                throw new InvalidOperationException("Invalid path to property");

            if (memberExpression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("Argument is not a property",
                    nameof(refToResourceInTargetResourceEntity));

            if (_bookStoreDbContext.Entry(targetResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(targetResourceEntity);
            }

            if (_bookStoreDbContext.Entry(refResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(refResourceEntity);
            }

            propertyInfo.SetValue(targetResourceEntity, refResourceEntity);
        }
        // public IEnumerable<TResourceEntity> Read(
        //     Expression<Func<TResourceEntity, bool>> predicate) =>
        //     _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .Where(predicate);

        // public IEnumerable<TResultData> Read<TResultData>(
        //     Expression<Func<TResultData, bool>> predicate)
        //     => _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .Where(predicate)
        //         .ProjectTo<TResultData>(_mapperConfigurationProvider);

        // public async Task<IEnumerable<TResourceEntity>> ReadAsync(
        //     Expression<Func<TResourceEntity, bool>> predicate)
        //     => await _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .Where(predicate)
        //         .ToArrayAsync();

        // public async Task<IEnumerable<TResultData>> ReadAsync<TResultData>(
        //     Expression<Func<TResourceEntity, bool>> predicate)
        //     => await _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .Where(predicate)
        //         .ProjectTo<TResultData>(_mapperConfigurationProvider)
        //         .ToArrayAsync();

        // public IEnumerable<TResultData> Read<TResultData>() =>
        //     _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .ProjectTo<TResultData>(_mapperConfigurationProvider);

        // public async Task<IEnumerable<TResultData>> ReadAsync<TResultData>() =>
        //     await _readSettings(
        //             _targetResourceCollectionSelector(_baseResourceEntityCollection))
        //         .ProjectTo<TResultData>(_mapperConfigurationProvider)
        //         .ToArrayAsync();

        // public async Task<bool> IsCreatedAsync(Expression<Func<TEntity, bool>> predicate)
        // {
        //     return await _entities.AsNoTracking().AnyAsync(predicate);
        // }
    }
}
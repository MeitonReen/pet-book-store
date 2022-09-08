using System.Linq.Expressions;
using System.Reflection;
using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner
{
    public class BaseResourceByEfCore<TResourceEntity> : IBaseResource<TResourceEntity>
        where TResourceEntity : class, new()
    {
        private readonly DbSet<TResourceEntity> _baseResourceEntityCollection;
        private readonly BaseBookStoreDbContext _bookStoreDbContext;

        private readonly Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _targetResourceEntitySelector;

        private Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
            _readSettings;

        private TResourceEntity? _resourceEntity;

        public BaseResourceByEfCore(BaseBookStoreDbContext bookStoreDbContext,
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>?
                targetResourceEntitySelector = default)
        {
            _bookStoreDbContext = bookStoreDbContext;
            _baseResourceEntityCollection = _bookStoreDbContext.Set<TResourceEntity>();

            _targetResourceEntitySelector = targetResourceEntitySelector
                                            ?? (_ => _baseResourceEntityCollection);
            _readSettings = readSettings => readSettings;
        }

        public TResourceEntity ResourceEntity =>
            _resourceEntity ?? throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

        public IBaseResource<TResourceEntity> Create(TResourceEntity targetResourceEntity)
        {
            _resourceEntity = targetResourceEntity;

            _baseResourceEntityCollection.Add(_resourceEntity);
            return this;
        }

        public IBaseResource<TResourceEntity> Create(Action<TResourceEntity> targetResourceEntitySettings)
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            return Create(targetResourceEntity);
        }

        // public async Task<TResourceEntity> CreateIfNotExists(
        //     TResourceEntity targetResourceEntity)
        // {
        //     if (!await _baseResourceEntityCollection.ContainsAsync(targetResourceEntity))
        //         _baseResourceEntityCollection.Add(targetResourceEntity);
        //
        //     return targetResourceEntity;
        // }

        public TResourceEntity? Read() =>
            _readSettings(
                    _targetResourceEntitySelector(
                        _baseResourceEntityCollection))
                .SingleOrDefault();

        public Task<TResourceEntity?> ReadAsync() =>
            _readSettings(
                    _targetResourceEntitySelector(
                        _baseResourceEntityCollection))
                .SingleOrDefaultAsync();


        public IBaseResource<TResourceEntity> ReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings)
        {
            _readSettings = readSettings;
            return this;
        }

        public IBaseResource<TResourceEntity> AddReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings)
        {
            _readSettings = readQuery => readSettings(_readSettings(readQuery));
            return this;
        }

        public IReadableDataFromResource<TResultDataItem>
            ReadSettings<TResultDataItem>(Func<IQueryable<TResourceEntity>,
                IQueryable<TResultDataItem>> readSettings) where TResultDataItem : class
            => new DataFromResourceByEfCore<TResultDataItem>
            (readSettings(
                _readSettings(
                    _targetResourceEntitySelector(_baseResourceEntityCollection))));

        public IReadableDataFromResource<TResultDataItem>
            AddReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>,
                    IQueryable<TResultDataItem>> readSettings) where TResultDataItem : class
            => new DataFromResourceByEfCore<TResultDataItem>
            (readSettings(
                _readSettings(
                    _targetResourceEntitySelector(_baseResourceEntityCollection))));


        public IBaseResource<TResourceEntity> Update(TResourceEntity targetResourceEntity)
        {
            _resourceEntity = targetResourceEntity;
            _baseResourceEntityCollection.Update(_resourceEntity);
            return this;
        }

        public IBaseResource<TResourceEntity> Update(
            TResourceEntity targetResourceEntity, Action<TResourceEntity> updateSettings)
        {
            _resourceEntity = targetResourceEntity;

            if (_bookStoreDbContext.Entry(_resourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(_resourceEntity);
            }

            updateSettings(_resourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> Update(
            Action<TResourceEntity> targetResourceEntitySettings, Action<TResourceEntity> updateSettings)
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            return Update(targetResourceEntity, updateSettings);
        }

        public IBaseResource<TResourceEntity> Update(Action<TResourceEntity> updateSettings)
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return Update(_resourceEntity, updateSettings);
        }

        public TResourceEntity Delete(TResourceEntity targetResourceEntity)
        {
            _resourceEntity = default;
            _baseResourceEntityCollection.Remove(targetResourceEntity);
            return targetResourceEntity;
        }

        public TResourceEntity Delete(Action<TResourceEntity> targetResourceEntitySettings)
        {
            var targetResource = new TResourceEntity();
            targetResourceEntitySettings(targetResource);

            return Delete(targetResource);
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            _resourceEntity = targetResourceEntity;

            var collectionInTargetResourceEntity = refToCollectionInTargetResourceEntity(
                targetResourceEntity);

            if (_bookStoreDbContext.Entry(targetResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(targetResourceEntity);
            }

            if (_bookStoreDbContext.Entry(refResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(refResourceEntity);
            }

            collectionInTargetResourceEntity.Add(refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>> refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return CreateReference(_resourceEntity, refToCollectionInTargetResourceEntity,
                refResourceEntity);
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var refResourceEntity = new TRefResourceEntity();
            refResourceEntitySettings(refResourceEntity);

            CreateReference(
                targetResourceEntity,
                refToCollectionInTargetResourceEntity,
                refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>> refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings) where TRefResourceEntity : class, new()
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return CreateReference(_resourceEntity, refToCollectionInTargetResourceEntity,
                refResourceEntitySettings);
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            CreateReference(
                targetResourceEntity,
                refToCollectionInTargetResourceEntity,
                refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            var refResourceEntity = new TRefResourceEntity();
            refResourceEntitySettings(refResourceEntity);

            CreateReference(
                targetResourceEntity,
                refToCollectionInTargetResourceEntity,
                refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class
        {
            _resourceEntity = targetResourceEntity;

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

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>> refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection) where TRefResourceEntity : class
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return CreateReferences(_resourceEntity, refToCollectionInTargetResourceEntity,
                refResourceEntityCollection);
        }

        public IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            CreateReferences(
                targetResourceEntity,
                refToCollectionInTargetResourceEntity,
                refResourceEntityCollection);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            if (refToResourceInTargetResourceEntity == default)
                throw new ArgumentNullException(nameof(
                    refToResourceInTargetResourceEntity));

            if (refToResourceInTargetResourceEntity.Body
                is not MemberExpression memberExpression)
                throw new InvalidOperationException("Invalid path to property");

            if (memberExpression.Member is not PropertyInfo propertyInfo)
                throw new ArgumentException("Argument is not a property",
                    nameof(refToResourceInTargetResourceEntity));

            _resourceEntity = targetResourceEntity;

            if (_bookStoreDbContext.Entry(targetResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(targetResourceEntity);
            }

            if (_bookStoreDbContext.Entry(refResourceEntity).State == EntityState.Detached)
            {
                _bookStoreDbContext.Attach(refResourceEntity);
            }

            propertyInfo.SetValue(targetResourceEntity, refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Expression<Func<TResourceEntity, TRefResourceEntity?>> refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity) where TRefResourceEntity : class, new()
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return CreateReference(_resourceEntity, refToResourceInTargetResourceEntity,
                refResourceEntity);
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var resourceInTargetResource = new TRefResourceEntity();
            refResourceEntitySettings(resourceInTargetResource);

            CreateReference(
                targetResourceEntity,
                refToResourceInTargetResourceEntity,
                resourceInTargetResource);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Expression<Func<TResourceEntity, TRefResourceEntity?>> refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings) where TRefResourceEntity : class, new()
        {
            if (_resourceEntity == default)
                throw new InvalidOperationException($"{nameof(ResourceEntity)} is null");

            return CreateReference(_resourceEntity, refToResourceInTargetResourceEntity,
                refResourceEntitySettings);
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            var refResourceEntity = new TRefResourceEntity();
            refResourceEntitySettings(refResourceEntity);

            CreateReference(
                targetResourceEntity,
                refToResourceInTargetResourceEntity,
                refResourceEntity);

            return this;
        }

        public IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new()
        {
            var targetResourceEntity = new TResourceEntity();
            targetResourceEntitySettings(targetResourceEntity);

            CreateReference(
                targetResourceEntity,
                refToResourceInTargetResourceEntity,
                refResourceEntity);

            return this;
        }

        public IQueryable<TResourceEntity> Query => _targetResourceEntitySelector(
            _baseResourceEntityCollection);
        // public async Task<bool> IsCreatedAsync(Expression<Func<TEntity, bool>> predicate)
        // {
        //     return await _entities.AsNoTracking().AnyAsync(predicate);
        // }
    }
}
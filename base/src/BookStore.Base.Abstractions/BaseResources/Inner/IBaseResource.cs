using System.Linq.Expressions;

namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IBaseResource<TResourceEntity>
        where TResourceEntity : class
    {
        TResourceEntity ResourceEntity { get; }

        IQueryable<TResourceEntity> Query { get; }
        IBaseResource<TResourceEntity> Create(TResourceEntity targetResourceEntity);

        IBaseResource<TResourceEntity> Create(Action<TResourceEntity> targetResourceEntitySettings);
        // Task<TResourceEntity> CreateIfNotExists(TResourceEntity targetResourceEntity);

        public IBaseResource<TResourceEntity> ReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings);

        IBaseResource<TResourceEntity> AddReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings);

        IReadableDataFromResource<TResultDataItem> ReadSettings<TResultDataItem>
            (Func<IQueryable<TResourceEntity>, IQueryable<TResultDataItem>> readSettings)
            where TResultDataItem : class;

        IReadableDataFromResource<TResultDataItem>
            AddReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>, IQueryable<TResultDataItem>> readSettings)
            where TResultDataItem : class;

        TResourceEntity? Read();
        Task<TResourceEntity?> ReadAsync();

        IBaseResource<TResourceEntity> Update(TResourceEntity targetResourceEntity);

        IBaseResource<TResourceEntity> Update(
            TResourceEntity targetResourceEntity, Action<TResourceEntity> updateSettings);

        IBaseResource<TResourceEntity> Update(Action<TResourceEntity>
            targetResourceEntitySettings, Action<TResourceEntity> updateSettings);

        IBaseResource<TResourceEntity> Update(Action<TResourceEntity> updateSettings);
        TResourceEntity Delete(TResourceEntity targetResourceEntity);
        TResourceEntity Delete(Action<TResourceEntity> targetResourceEntitySettings);

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class;

        IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class;

        IBaseResource<TResourceEntity> CreateReferences<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            TResourceEntity targetResourceEntity,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResource<TResourceEntity> CreateReference<TRefResourceEntity>(
            Action<TResourceEntity> targetResourceEntitySettings,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();
    }
}
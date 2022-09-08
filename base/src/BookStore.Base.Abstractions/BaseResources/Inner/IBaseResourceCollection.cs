using System.Linq.Expressions;

namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IBaseResourceCollection<TResourceEntity> where TResourceEntity : class
    {
        IQueryable<TResourceEntity> Query { get; }

        IEnumerable<TResourceEntity> Create(IEnumerable<TResourceEntity>
            targetResourceEntityCollection);

        IBaseResourceCollection<TResourceEntity> ReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings);

        IBaseResourceCollection<TResourceEntity> AddReadSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>> readSettings);

        IReadableDataCollectionFromResourceCollection<TResultDataItem>
            ReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>, IQueryable<TResultDataItem>> readSettings)
            where TResultDataItem : class;

        IReadableDataCollectionFromResourceCollection<TResultDataItem>
            AddReadSettings<TResultDataItem>(
                Func<IQueryable<TResourceEntity>, IQueryable<TResultDataItem>> readSettings)
            where TResultDataItem : class;

        IEnumerable<TResourceEntity> Read();
        Task<IEnumerable<TResourceEntity>> ReadAsync();

        IEnumerable<TResourceEntity> Update(
            IEnumerable<TResourceEntity> targetResourceEntityCollection);

        IEnumerable<TResourceEntity> Delete(
            IEnumerable<TResourceEntity> targetResourceEntityCollection);

        IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            TRefResourceEntity refResourceEntity)
            where TRefResourceEntity : class, new();

        IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        IBaseResourceCollection<TResourceEntity> CreateReferencesInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Func<TResourceEntity, ICollection<TRefResourceEntity>>
                refToCollectionInTargetResourceEntity,
            IEnumerable<TRefResourceEntity> refResourceEntityCollection)
            where TRefResourceEntity : class, new();

        IBaseResourceCollection<TResourceEntity> CreateReferenceInner<TRefResourceEntity>(
            IEnumerable<TResourceEntity> targetResourceEntityCollection,
            Expression<Func<TResourceEntity, TRefResourceEntity?>>
                refToResourceInTargetResourceEntity,
            Action<TRefResourceEntity> refResourceEntitySettings)
            where TRefResourceEntity : class, new();

        // IEnumerable<TResourceEntity> Read(
        //     Expression<Func<TResourceEntity, bool>> predicate);
        //
        // IEnumerable<TResultDataItem> Read<TResultDataItem>(
        //     Expression<Func<TResultDataItem, bool>> predicate,
        //     Func<IQueryable<TResourceEntity>, IEnumerable<TResultDataItem>>
        //         projectIQueryableConfigurator);

        // Task<IEnumerable<TResourceEntity>> ReadAsync(
        //     Expression<Func<TResourceEntity, bool>> predicate);
        //
        // Task<IEnumerable<TResultDataItem>> ReadAsync<TResultDataItem>(
        //     Expression<Func<TResultDataItem, bool>> predicate,
        //     Func<IQueryable<TResourceEntity>, IEnumerable<TResultDataItem>>
        //         projectIQueryableConfigurator);

        // IEnumerable<TResultDataItem> Read<TResultDataItem>(
        //     Func<IQueryable<TResourceEntity>, IEnumerable<TResultDataItem>>
        //         projectIQueryableConfigurator);
        // Task<IEnumerable<TResultDataItem>> ReadAsync<TResultDataItem>(
        //     Func<IQueryable<TResourceEntity>, IEnumerable<TResultDataItem>>
        //         projectIQueryableConfigurator);
    }
}
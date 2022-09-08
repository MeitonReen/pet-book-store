namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IBaseResourceCollectionCount<TResourceEntity>
        where TResourceEntity : class
    {
        IQueryable<TResourceEntity> Query { get; }
        Task<int> ReadAsync();
        Task<long> ReadLongAsync();

        public IBaseResourceCollectionCount<TResourceEntity> ResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                resourceCollectionSettings);

        IBaseResourceCollectionCount<TResourceEntity> AddResourceCollectionSettings(
            Func<IQueryable<TResourceEntity>, IQueryable<TResourceEntity>>
                resourceCollectionSettings);
    }
}
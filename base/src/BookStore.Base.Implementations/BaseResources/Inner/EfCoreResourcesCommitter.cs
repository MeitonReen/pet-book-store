using BookStore.Base.Abstractions.BaseResources.Inner;

namespace BookStore.Base.Implementations.BaseResources.Inner
{
    public class EfCoreResourcesCommitter : IResourcesCommitter
    {
        private readonly BaseBookStoreDbContext _bookStoreDbContext;

        public EfCoreResourcesCommitter(BaseBookStoreDbContext bookStoreDbContext)
        {
            _bookStoreDbContext = bookStoreDbContext;
        }

        public Task CommitAsync() => _bookStoreDbContext.SaveChangesAsync();
    }
}
namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IResourcesCommitter
    {
        Task CommitAsync();
    }
}
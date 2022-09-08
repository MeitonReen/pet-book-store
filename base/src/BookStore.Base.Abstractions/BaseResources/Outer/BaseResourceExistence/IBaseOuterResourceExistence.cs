namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence
{
    public interface IBaseOuterResourceExistence
    {
        Func<Task<bool>> Query { get; }
        Task<bool> ReadAsync();

        IBaseOuterResourceExistence ReadSettings(Action<
            IBaseOuterResourceExistenceReadSettings> readSettings);
    }
}
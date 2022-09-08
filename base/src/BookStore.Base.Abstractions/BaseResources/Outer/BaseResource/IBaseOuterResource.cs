using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;

namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResource
{
    public interface IBaseOuterResource<TResourceEntity>
        where TResourceEntity : class
    {
        Func<Task<TResourceEntity?>> Query { get; }
        Task<TResourceEntity?> ReadAsync();

        public IBaseOuterResource<TResourceEntity> ReadSettings(
            Action<IBaseOuterResourceReadSettingsStep1<TResourceEntity>> readSettings);
    }
}
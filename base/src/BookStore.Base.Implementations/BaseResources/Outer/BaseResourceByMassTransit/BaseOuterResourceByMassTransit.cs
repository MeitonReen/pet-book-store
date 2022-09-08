using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;
using BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit.ReadSettingsSteps;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit
{
    public class BaseOuterResourceByMassTransit<TResourceEntity>
        : IBaseOuterResource<TResourceEntity>
        where TResourceEntity : class
    {
        private readonly IBaseOuterResourceReadSettingsStep1<TResourceEntity> _readSettings;

        public BaseOuterResourceByMassTransit(
            IServiceProvider serviceProvider
        )
        {
            _readSettings = new BaseOuterResourceReadSettingsStep1ByMassTransit<
                TResourceEntity>(serviceProvider, query => Query = query);
        }

        public Func<Task<TResourceEntity?>> Query { get; private set; } = () =>
            throw new InvalidOperationException(
                $"{typeof(IBaseOuterResource<TResourceEntity>).Name} is not completely configured" +
                $"in {nameof(ReadSettings)}");

        public Task<TResourceEntity?> ReadAsync() => Query();

        public IBaseOuterResource<TResourceEntity> ReadSettings(
            Action<IBaseOuterResourceReadSettingsStep1<TResourceEntity>> readSettings)
        {
            readSettings(_readSettings);
            return this;
        }
    }
}
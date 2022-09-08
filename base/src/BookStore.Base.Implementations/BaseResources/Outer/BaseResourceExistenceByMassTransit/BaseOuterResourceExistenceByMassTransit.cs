using BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceExistenceByMassTransit
{
    public class BaseOuterResourceExistenceByMassTransit
        : IBaseOuterResourceExistence
    {
        private readonly IBaseOuterResourceExistenceReadSettings _readSettings;

        public BaseOuterResourceExistenceByMassTransit(
            IServiceProvider serviceProvider
        )
        {
            _readSettings = new BaseOuterResourceExistenceReadSettingsByMassTransit(
                serviceProvider, query => Query = query);
        }

        public Func<Task<bool>> Query { get; private set; } = () =>
            throw new InvalidOperationException(
                $"{nameof(IBaseOuterResourceExistence)} is not completely configured" +
                $"in {nameof(ReadSettings)}");

        public Task<bool> ReadAsync() => Query();

        public IBaseOuterResourceExistence ReadSettings(
            Action<IBaseOuterResourceExistenceReadSettings> readSettings)
        {
            readSettings(_readSettings);
            return this;
        }
    }
}
using MassTransit;

namespace BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit.Extensions
{
    public static class RabbitMqBusFactoryConfiguratorExtensions
    {
        public static void UseBookStoreDefaultLogging(this IRabbitMqBusFactoryConfigurator
            busFactoryConfigurator, IBusRegistrationContext context)
        {
            busFactoryConfigurator.UsePublishFilter(
                typeof(LoggingFilters<>), context);
            busFactoryConfigurator.UseSendFilter(
                typeof(LoggingFilters<>), context);
            busFactoryConfigurator.UseConsumeFilter(
                typeof(LoggingFilters<>), context);
        }
    }
}
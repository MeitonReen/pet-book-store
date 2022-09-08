using MassTransit;

namespace BookStore.Base.Implementations.CorrelationId.MassTransitSupport.Extensions
{
    public static class RabbitMqBusFactoryConfiguratorExtensions
    {
        public static void UseCorrelationId(this IRabbitMqBusFactoryConfigurator
            busFactoryConfigurator, IBusRegistrationContext context)
        {
            busFactoryConfigurator.UsePublishFilter(
                typeof(CorrelationIdPublishAndSendFilter<>), context);
            busFactoryConfigurator.UseSendFilter(
                typeof(CorrelationIdPublishAndSendFilter<>), context);
            busFactoryConfigurator.UseConsumeFilter(
                typeof(CorrelationIdConsumeFilter<>), context);
        }
    }
}
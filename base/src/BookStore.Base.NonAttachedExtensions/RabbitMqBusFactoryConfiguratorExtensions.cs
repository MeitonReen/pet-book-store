using DateOnlyTimeOnly.AspNet.Converters;
using MassTransit;

namespace BookStore.Base.NonAttachedExtensions
{
    public static class RabbitMqBusFactoryConfiguratorExtensions
    {
        public static void MassTransitDateOnlyTimeOnlyTempFix(
            this IRabbitMqBusFactoryConfigurator
                rabbitMqBusFactoryConfigurator)
        {
            rabbitMqBusFactoryConfigurator
                .ConfigureJsonSerializerOptions(sets =>
                {
                    if (!sets.Converters.Any(b => b.CanConvert(typeof(DateOnly))))
                    {
                        sets.Converters.Add(new DateOnlyJsonConverter());
                    }

                    if (!sets.Converters.Any(b => b.CanConvert(typeof(TimeOnly))))
                    {
                        sets.Converters.Add(new TimeOnlyJsonConverter());
                    }

                    return sets;
                });
        }
    }
}
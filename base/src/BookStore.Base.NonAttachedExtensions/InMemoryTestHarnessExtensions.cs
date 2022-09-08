using DateOnlyTimeOnly.AspNet.Converters;
using MassTransit;
using MassTransit.Testing;

namespace BookStore.Base.NonAttachedExtensions
{
    public static class InMemoryTestHarnessExtensions
    {
        public static void MassTransitDateOnlyTimeOnlyTempFix(this InMemoryTestHarness
            inMemoryTestHarness)
        {
            inMemoryTestHarness.OnConfigureInMemoryBus += inMemoryBusFactoryConfigurator =>
                inMemoryBusFactoryConfigurator
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
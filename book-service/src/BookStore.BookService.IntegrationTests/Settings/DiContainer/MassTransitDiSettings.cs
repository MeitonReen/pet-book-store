using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.Masstransit.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit.Extensions;
using BookStore.Base.NonAttachedExtensions;
using BookStore.BookService.Data.BaseDatabase;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.BookService.IntegrationTests.Settings.DiContainer;

public static class MassTransitDiSettings
{
    public static IServiceCollection AddMassTransitSettings(
        this IServiceCollection services,
        MasstransitConfig masstransitConfig)
        => services.AddMassTransitTestHarness(sets =>
        {
            // sets.AddDefaultTransactionOutbox<BaseDbContext>();

            // sets.AddConsumer<BookDataByOrderServiceReadRequestConsumer>();
            // sets.AddConsumer<ReadBookExistenceRequestConsumer>();

            sets.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(true));

            sets.UsingRabbitMq((context, rabbitMqSets) =>
            {
                // rabbitMqSets.UseCorrelationId(context);
                rabbitMqSets.UseBookStoreDefaultLogging(context);
                rabbitMqSets.UseBookStoreDefaultExceptionHandling();

                rabbitMqSets.MassTransitDateOnlyTimeOnlyTempFix();

                rabbitMqSets.Host(masstransitConfig.Uri, masstransitConfig.Host,
                    hostSets =>
                    {
                        hostSets.Username(masstransitConfig.UserName);
                        hostSets.Password(masstransitConfig.Password);
                    });
                rabbitMqSets.ConfigureEndpoints(context);
            });
        });

// services.AddScoped<InMemoryTestHarness>(serviceProvider =>
// {
//     var massTransitTestHarness = new InMemoryTestHarness();
//
//     massTransitTestHarness.Consumer(() =>
//         new OrderServiceUnknownBookRequestConsumer(
//             serviceProvider.GetRequiredService<IUnitOfWork>(),
//             serviceProvider.GetRequiredService<IMapper>(),
//             serviceProvider.GetRequiredService<ILogger<OrderServiceUnknownBookRequestConsumer>>()));
//
//     massTransitTestHarness.Consumer(() =>
//         new UserServiceUnknownBookRequestConsumer(
//             serviceProvider.GetRequiredService<IUnitOfWork>(),
//             serviceProvider.GetRequiredService<IMapper>(),
//             serviceProvider.GetRequiredService<ILogger<OrderServiceUnknownBookRequestConsumer>>()));
//
//     massTransitTestHarness.TestTimeout = TimeSpan.FromSeconds(5);
//     massTransitTestHarness.TestInactivityTimeout = TimeSpan.FromSeconds(2);
//
//     massTransitTestHarness.Start();
//
//     return massTransitTestHarness;
// });
//
// services.AddScoped<IRequestClient<OrderServiceUnknownBookRequest>>(
//     serviceProvider =>
//     {
//         var massTransitTestHarness = serviceProvider
//             .GetRequiredService<InMemoryTestHarness>();
//
//         return massTransitTestHarness.CreateRequestClient<OrderServiceUnknownBookRequest>();
//     });
// services.AddScoped<IRequestClient<UserServiceUnknownBookRequest>>(
//     serviceProvider =>
//     {
//         var massTransitTestHarness = serviceProvider
//             .GetRequiredService<InMemoryTestHarness>();
//
//         return massTransitTestHarness.CreateRequestClient<UserServiceUnknownBookRequest>();
//     });
//
// services.AddScoped<IPublishEndpoint>(serviceProvider =>
// {
//     var massTransitTestHarness = serviceProvider
//         .GetRequiredService<InMemoryTestHarness>();
//     return massTransitTestHarness.Bus;
// });
//
// services
//     .AddMassTransit(busRegistrationSettings)
//     .AddMassTransit(sets =>
//     {
//         // var schedulerQueueName = "scheduler";
//         // var schedulerEndpoint = new Uri($"queue:{schedulerQueueName}");
//         //
//         // sets.AddMessageScheduler(schedulerEndpoint);
//         // sets.AddConsumer<BookUpdatedMessageConsumer>();
//         sets.AddDefaultTransactionOutbox<BaseDbContext>();
//
//         sets.AddConsumer<BookDataByOrderServiceReadRequestConsumer>();
//         sets.AddConsumer<BookPresenceInResourceCollectionReadRequestConsumer>();
//
//         sets.SetKebabCaseEndpointNameFormatter();
//
//         sets.UsingRabbitMq((context, rabbitMqSets) =>
//         {
//             rabbitMqSets.UseCorrelationId(context);
//             rabbitMqSets.UseBookStoreDefaultLogging(context);
//             rabbitMqSets.UseBookStoreDefaultExceptionHandling();
//
//             rabbitMqSets.MassTransitDateOnlyTimeOnlyTempFix();
//
//             rabbitMqSets.Host(masstransitConfig.Uri, masstransitConfig.Host,
//                 hostSets =>
//                 {
//                     hostSets.Username(masstransitConfig.UserName);
//                     hostSets.Password(masstransitConfig.Password);
//                 });
//             rabbitMqSets.ConfigureEndpoints(context);
//         });
//     });
}
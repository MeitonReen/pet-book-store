using AutoFixture;
using AutoFixture.AutoMoq;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BookStore.OrderService.IntegrationTests.Settings.DiContainer;

public static class UserClaimsProfileStubDiSettings
{
    public static IServiceCollection AddUserClaimsProfileStubSettings(
        this IServiceCollection services)
    {
        var userProfileMock = new Fixture()
            .Customize(new AutoMoqCustomization())
            .Create<Mock<IUserClaimsProfile>>();

        userProfileMock.Setup(path => path.UserId)
            .Returns(InitDataCreator.Profiles.TestUser.UserId.ToString());

        return services.AddScoped(_ => userProfileMock.Object);
    }
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
//     massTransitTestHarness.Start().Wait();
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
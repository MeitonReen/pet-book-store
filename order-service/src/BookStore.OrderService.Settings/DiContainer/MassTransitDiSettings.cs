using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.Masstransit.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit.Extensions;
using BookStore.Base.Implementations.CorrelationId.MassTransitSupport.Extensions;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.Base.NonAttachedExtensions;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.Contracts.Book.V1_0_0.Delete;
using BookStore.OrderService.Contracts.Book.V1_0_0.Update;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;
using BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;
using BookStore.OrderService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.OrderService.Settings.DiContainer;

public static class MassTransitDiSettings
{
    public static IServiceCollection AddMassTransitSettings(
        this IServiceCollection services,
        MasstransitConfig masstransitConfig,
        Action<IBusRegistrationConfigurator>? busRegistrationSettings = default)
        => services
            .AddMassTransit(busRegistrationSettings)
            .AddMassTransit(sets =>
            {
                // var schedulerQueueName = "scheduler";
                // var schedulerEndpoint = new Uri($"queue:{schedulerQueueName}");
                //
                // sets.AddMessageScheduler(schedulerEndpoint);
                // sets.AddDefaultTransactionOutbox<BaseDbContext>();

                // sets.AddConsumer<BookUpdatedMessageConsumer>();
                // sets.AddConsumer<BookDeletedMessageConsumer>();
                // sets.AddConsumer<ProfileDeletedConsumer>();

                sets.AddActivity<UpdateBookCommandActivity, UpdateBookCommand,
                    UpdateBookCompensateCommand, UpdateBookCommandActivityDefinition>();

                sets.AddActivity<DeleteBookCommandActivity, DeleteBookCommand,
                    DeleteBookCompensateCommand, DeleteBookCommandActivityDefinition>();

                sets.AddActivity<DeleteProfileCommandActivity, DeleteProfileCommand, Profile,
                    DeleteProfileCommandActivityDefinition>();

                sets.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(true));

                sets.UsingRabbitMq((context, rabbitMqSets) =>
                {
                    rabbitMqSets.UseCorrelationId(context);
                    rabbitMqSets.UseBookStoreDefaultLogging(context);
                    rabbitMqSets.UseBookStoreDefaultExceptionHandling();

                    rabbitMqSets.MassTransitDateOnlyTimeOnlyTempFix();
                    // rabbitMqSets.ConfigureJsonSerializerOptions(setsJsonSer =>
                    //     {
                    //         setsJsonSer.ReferenceHandler = ReferenceHandler.Preserve;
                    //         return setsJsonSer;
                    //     }
                    // );
                    rabbitMqSets.Host(masstransitConfig.Uri, masstransitConfig.Host,
                        hostSets =>
                        {
                            hostSets.Username(masstransitConfig.UserName);
                            hostSets.Password(masstransitConfig.Password);
                        });
                    rabbitMqSets.ConfigureEndpoints(context);
                });
            });
}
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.Masstransit.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit.Extensions;
using BookStore.Base.Implementations.CorrelationId.MassTransitSupport.Extensions;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.DeleteOut;
using BookStore.Base.NonAttachedExtensions;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance;
using BookStore.UserService.Data.SagasDatabase;
using BookStore.UserService.Settings.Resources.Book.V1_0_0.Delete;
using BookStore.UserService.Settings.Resources.Book.V1_0_0.DeleteOut;
using BookStore.UserService.Settings.Resources.Profile.V1_0_0.Delete;
using BookStore.UserService.Settings.Resources.ProfileExistence.V1_0_0.Read;
using BookStore.UserService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;
using BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;
using BookStore.UserService.WebEntryPoint.Profile.V1_0_0.DeleteOut;
using BookStore.UserService.WebEntryPoint.ProfileExistence.V1_0_0.Read;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.UserService.Settings.DiContainer;

public static class MassTransitDiSettings
{
    public static IServiceCollection AddMassTransitSettings(
        this IServiceCollection services,
        MasstransitConfig masstransitConfig,
        Action<IBusRegistrationConfigurator>? busRegistrationSettings = default)
        => services.AddMassTransit(busRegistrationSettings)
            .AddMassTransit(sets =>
            {
                // var schedulerQueueName = "scheduler";
                // var schedulerEndpoint = new Uri($"queue:{schedulerQueueName}");
                //
                // sets.AddMessageScheduler(schedulerEndpoint);
                sets.AddDefaultTransactionOutbox<BaseDbContext>();
                sets.AddDefaultTransactionOutbox<SagasDbContext>();

                sets.AddConsumer<ReadProfileExistenceRequestConsumer,
                    ReadProfileExistenceRequestConsumerDefinition>();

                sets.AddRequestClient<DeleteProfileRequest>();

                sets.AddActivity<DeleteBookCommandActivity, DeleteBookCommand, Book,
                    DeleteBookCommandActivityDefinition>();
                sets.AddActivity<DeleteProfileCommandActivity, DeleteProfileCommand,
                    Profile, DeleteProfileCommandActivityDefinition>();

                sets
                    .AddSagaStateMachine<SagaOrchestrator, SagaOrchestratorInstance,
                    SagaOrchestratorDefinition>()
                    .EntityFrameworkRepository(efRepSets =>
                {
                    efRepSets.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    efRepSets.UsePostgres();
                    efRepSets.ExistingDbContext<SagasDbContext>();
                });

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
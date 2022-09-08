using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit.Extensions;
using BookStore.Base.Implementations.CorrelationId.MassTransitSupport.Extensions;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;
using BookStore.Base.NonAttachedExtensions;
using BookStore.BookService.BL.ResourceEntities;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance;
using BookStore.BookService.Data.SagasDatabase;
using BookStore.BookService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;
using BookStore.BookService.WebEntryPoint.Book.V1_0_0.DeleteOut;
using BookStore.BookService.WebEntryPoint.Book.V1_0_0.Read;
using BookStore.BookService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;
using BookStore.BookService.WebEntryPoint.BookExistence.V1_0_0.Read;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.Settings.DiContainer;

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
                // sets.AddDefaultTransactionOutbox<BaseDbContext>();
                sets.AddDefaultTransactionOutbox<SagasDbContext>();

                sets.AddRequestClient<UpdateBookRequest>();
                sets.AddRequestClient<DeleteBookRequest>();

                sets.AddConsumer<ReadBookRequestFromOrderServiceConsumer>();
                sets.AddConsumer<ReadBookExistenceRequestConsumer>();

                sets.AddActivity<UpdateBookCommandActivity, UpdateBookCommand, UpdateBookCompensateCommand,
                    UpdateBookCommandActivityDefinition>();

                sets.AddActivity<DeleteBookCommandActivity, DeleteBookCommand, Book,
                    DeleteBookCommandActivityDefinition>();

                sets.AddSagaStateMachine<SagaOrchestrator, SagaOrchestratorInstance,
                    SagaOrchestratorInstanceDefinition>().EntityFrameworkRepository(efRepSets =>
                {
                    efRepSets.ConcurrencyMode = ConcurrencyMode.Optimistic;
                    efRepSets.UsePostgres();
                    efRepSets.ExistingDbContext<SagasDbContext>();
                });

                sets.AddSagaStateMachine<WebEntryPoint.Book.V1_0_0.UpdateOut.SagaOrchestrator,
                        Data.Book.V1_0_0.UpdateOut.SagaInstance.SagaOrchestratorInstance,
                        Data.Book.V1_0_0.UpdateOut.SagaInstance.SagaOrchestratorInstanceDefinition>()
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
                    // rabbitMqSets.UseBookStoreDefaultExceptionHandling();

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
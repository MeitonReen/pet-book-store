using AutoMapper;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut;
using BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.DeleteOut;

public class SagaOrchestrator : MassTransitStateMachine<SagaOrchestratorInstance>
{
    private static readonly string BookServiceDeleteBookCommandActivityName =
        $"{nameof(BookService)}Courier"
        + $"-->{nameof(DeleteBookCommand)}"
        + $"-->{DeleteBookCommandActivityContracts.ExecuteEndpoint.Name}";

    private static readonly string OrderServiceDeleteBookCommandActivityName =
        $"{nameof(BookService)}Courier"
        + "-->"
        + nameof(Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommand)
        + "-->"
        + Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommandActivityContracts
            .ExecuteEndpoint.Name;

    private static readonly string UserServiceDeleteBookCommandActivityName =
        $"{nameof(BookService)}Courier"
        + "-->"
        + nameof(Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommand)
        + "-->"
        + Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommandActivityContracts
            .ExecuteEndpoint.Name;

    private readonly IServiceProvider _serviceProvider;

    public SagaOrchestrator(IEndpointNameFormatter endpointNameFormatter,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InstanceState(sets => sets.CurrentState, Started);

        Event(() => DeletingBookStarted, sets => sets.CorrelateById(context =>
            // context.Message.BookId
            context.RequestId ??
            throw new InvalidOperationException("Received message is not request")));

        Event(() => DeleteBookSuccess, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Event(() => DeleteBookFailed, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Initially(
            When(DeletingBookStarted)
                .Then(context => InitOrchestratorInstance(context, endpointNameFormatter))
                .ThenAsync(CourierStart)
                .TransitionTo(Started));

        During(Started,
            When(DeleteBookSuccess)
                .ThenAsync(RespondSuccess)
                .TransitionTo(Success)
                .Finalize());

        During(Started,
            When(DeleteBookFailed)
                .ThenAsync(RespondFailed)
                .TransitionTo(Failed)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    public State Started { get; private set; }
    public State Success { get; private set; }
    public State Failed { get; private set; }

    public Event<DeleteBookRequest> DeletingBookStarted { get; private set; }
    public Event<RoutingSlipCompleted> DeleteBookSuccess { get; private set; }
    public Event<RoutingSlipFaulted> DeleteBookFailed { get; private set; }

    private void InitOrchestratorInstance(BehaviorContext<SagaOrchestratorInstance> context,
        IEndpointNameFormatter endpointNameFormatter)
    {
        context.Saga.OrchestratorInstanceAddress =
            new Uri($"exchange:{endpointNameFormatter.Saga<SagaOrchestratorInstance>()}");
    }

    private async Task CourierStart(BehaviorContext<SagaOrchestratorInstance,
        DeleteBookRequest> context
    )
    {
        var sagaConsumeContext = context.GetPayload<ConsumeContext>();

        context.Saga.RequestId =
            sagaConsumeContext.RequestId ??
            throw new InvalidOperationException("Received message is not request");

        context.Saga.ResponseAddress =
            sagaConsumeContext.ResponseAddress ??
            throw new InvalidOperationException("Response address not found");

        await sagaConsumeContext.Execute(CreateRoutingSlip(context.Message,
            context.Saga.OrchestratorInstanceAddress,
            context.Saga.CorrelationId));
    }

    private async Task RespondSuccess(
        BehaviorContext<SagaOrchestratorInstance, RoutingSlipCompleted> context)
        => await context.Send<Success>(context.Saga.ResponseAddress, new { }, sets =>
            sets.RequestId = context.Saga.RequestId);

    private async Task RespondFailed(
        BehaviorContext<SagaOrchestratorInstance, RoutingSlipFaulted> context)
        => await context.Send<Failed>(context.Saga.ResponseAddress, new { }, sets =>
            sets.RequestId = context.Saga.RequestId);

    private RoutingSlip CreateRoutingSlip(DeleteBookRequest request,
        Uri targetSagaAddress,
        Guid trackingNumber)
    {
        var mapper = _serviceProvider.GetRequiredService<IMapper>();

        var builder = new RoutingSlipBuilder(trackingNumber);

        AddBookServiceDeleteBookCommandActivity(builder, request);

        AddOrderServiceDeleteBookCommandActivity(builder, mapper, request);

        AddUserServiceDeleteBookCommandActivity(builder, mapper, request);

        builder.AddSubscription(targetSagaAddress,
            RoutingSlipEvents.Completed,
            RoutingSlipEventContents.None,
            sets => sets.Send<RoutingSlipCompleted>(new {builder.TrackingNumber}));
        ;
        builder.AddSubscription(targetSagaAddress,
            RoutingSlipEvents.Faulted,
            RoutingSlipEventContents.None,
            sets => sets.Send<RoutingSlipFaulted>(new {builder.TrackingNumber}));

        return builder.Build();
    }

    private void AddBookServiceDeleteBookCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        DeleteBookRequest request)
    {
        DeleteBookCommand deleteBookCommandToBookService = request;

        routingSlipBuilder.AddActivity(
            BookServiceDeleteBookCommandActivityName,
            DeleteBookCommandActivityContracts.ExecuteEndpoint.Uri,
            deleteBookCommandToBookService);
    }

    private void AddOrderServiceDeleteBookCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        IMapper mapper,
        DeleteBookRequest request)
    {
        var deleteBookCommandToOrderService = mapper
            .Map<Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommand>(request);

        routingSlipBuilder.AddActivity(
            OrderServiceDeleteBookCommandActivityName,
            Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommandActivityContracts
                .ExecuteEndpoint.Uri,
            deleteBookCommandToOrderService);
    }

    private void AddUserServiceDeleteBookCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        IMapper mapper,
        DeleteBookRequest request)
    {
        var deleteBookCommandToUserService = mapper
            .Map<Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommand>(request);

        routingSlipBuilder.AddActivity(
            UserServiceDeleteBookCommandActivityName,
            Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete.DeleteBookCommandActivityContracts
                .ExecuteEndpoint.Uri,
            deleteBookCommandToUserService);
    }
}
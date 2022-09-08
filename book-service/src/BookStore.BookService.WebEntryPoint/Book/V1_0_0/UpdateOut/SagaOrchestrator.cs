using AutoMapper;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;
using BookStore.BookService.Data.Book.V1_0_0.UpdateOut.SagaInstance;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.UpdateOut;

public class SagaOrchestrator : MassTransitStateMachine<SagaOrchestratorInstance>
{
    private static readonly string BookServiceUpdateBookCommandActivityName =
        $"{nameof(BookService)}Courier"
        + $"-->{nameof(UpdateBookCommand)}"
        + $"-->{UpdateBookCommandActivityContracts.ExecuteEndpoint.Name}";

    private static readonly string OrderServiceUpdateBookCommandActivityName =
        $"{nameof(BookService)}Courier"
        + "-->"
        + nameof(Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand)
        + "-->"
        + Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommandActivityContracts
            .ExecuteEndpoint.Name;

    private readonly IServiceProvider _serviceProvider;

    public SagaOrchestrator(IEndpointNameFormatter endpointNameFormatter,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InstanceState(sets => sets.CurrentState, Started);

        Event(() => UpdatingBookStarted, sets => sets.CorrelateById(context =>
            // context.Message.BookId
            context.RequestId ??
            throw new InvalidOperationException("Received message is not request")));

        Event(() => UpdateBookSuccess, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Event(() => UpdateBookFailed, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Initially(
            When(UpdatingBookStarted)
                .Then(context => InitOrchestratorInstance(context, endpointNameFormatter))
                .ThenAsync(CourierStart)
                .TransitionTo(Started));

        During(Started,
            When(UpdateBookSuccess)
                .ThenAsync(RespondSuccess)
                .TransitionTo(Success)
                .Finalize());

        During(Started,
            When(UpdateBookFailed)
                .ThenAsync(RespondFailed)
                .TransitionTo(Failed)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    public State Started { get; private set; }
    public State Success { get; private set; }
    public State Failed { get; private set; }

    public Event<UpdateBookRequest> UpdatingBookStarted { get; private set; }
    public Event<RoutingSlipCompleted> UpdateBookSuccess { get; private set; }
    public Event<RoutingSlipFaulted> UpdateBookFailed { get; private set; }

    private void InitOrchestratorInstance(BehaviorContext<SagaOrchestratorInstance> context,
        IEndpointNameFormatter endpointNameFormatter)
    {
        context.Saga.OrchestratorInstanceAddress =
            new Uri($"exchange:{endpointNameFormatter.Saga<SagaOrchestratorInstance>()}");
    }

    private async Task CourierStart(BehaviorContext<SagaOrchestratorInstance,
        UpdateBookRequest> context)
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

    private RoutingSlip CreateRoutingSlip(UpdateBookRequest request,
        Uri targetSagaAddress,
        Guid trackingNumber)
    {
        var mapper = _serviceProvider.GetRequiredService<IMapper>();

        var builder = new RoutingSlipBuilder(trackingNumber);

        AddBookServiceUpdateBookCommandActivity(builder, request);

        AddOrderServiceUpdateBookCommandActivity(builder, mapper, request);

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

    private void AddBookServiceUpdateBookCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        UpdateBookRequest request)
    {
        UpdateBookCommand updateBookCommandToBookService = request;

        routingSlipBuilder.AddActivity(
            BookServiceUpdateBookCommandActivityName,
            UpdateBookCommandActivityContracts.ExecuteEndpoint.Uri,
            updateBookCommandToBookService);
    }

    private void AddOrderServiceUpdateBookCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        IMapper mapper,
        UpdateBookRequest request)
    {
        var updateBookCommandToOrderService = mapper
            .Map<Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand>(request);

        routingSlipBuilder.AddActivity(
            OrderServiceUpdateBookCommandActivityName,
            Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommandActivityContracts
                .ExecuteEndpoint.Uri,
            updateBookCommandToOrderService);
    }
}
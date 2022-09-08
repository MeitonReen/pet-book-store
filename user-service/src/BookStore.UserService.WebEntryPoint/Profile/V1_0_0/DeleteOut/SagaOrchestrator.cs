using AutoMapper;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Profile.V1_0_0.DeleteOut;
using BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance;
using MassTransit;
using MassTransit.Courier.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.UserService.WebEntryPoint.Profile.V1_0_0.DeleteOut;

public class SagaOrchestrator : MassTransitStateMachine<SagaOrchestratorInstance>
{
    private static readonly string UserServiceDeleteProfileCommandActivityName =
        $"{nameof(UserService)}Courier"
        + $"-->{nameof(DeleteProfileCommand)}"
        + $"-->{DeleteProfileCommandActivityContracts.ExecuteEndpoint.Name}";

    private static readonly string OrderServiceDeleteProfileCommandActivityName =
        $"{nameof(UserService)}Courier"
        + "-->"
        + nameof(Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete.DeleteProfileCommand)
        + "-->"
        + Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete.DeleteProfileCommandActivityContracts
            .ExecuteEndpoint.Name;

    private readonly IServiceProvider _serviceProvider;

    public SagaOrchestrator(IEndpointNameFormatter endpointNameFormatter,
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InstanceState(sets => sets.CurrentState, Started);

        Event(() => DeletingProfileStarted, sets => sets.CorrelateById(context =>
            context.RequestId ??
            throw new InvalidOperationException("Received message is not request")));

        Event(() => DeleteProfileSuccess, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Event(() => DeleteProfileFailed, sets => sets.CorrelateById(context =>
            context.Message.TrackingNumber));

        Initially(
            When(DeletingProfileStarted)
                .Then(context => InitOrchestratorInstance(context, endpointNameFormatter))
                .ThenAsync(CourierStart)
                .TransitionTo(Started));

        During(Started,
            When(DeleteProfileSuccess)
                .ThenAsync(RespondSuccess)
                .TransitionTo(Success)
                .Finalize());

        During(Started,
            When(DeleteProfileFailed)
                .ThenAsync(RespondFailed)
                .TransitionTo(Failed)
                .Finalize());

        SetCompletedWhenFinalized();
    }

    public State Started { get; private set; }
    public State Success { get; private set; }
    public State Failed { get; private set; }

    public Event<DeleteProfileRequest> DeletingProfileStarted { get; private set; }
    public Event<RoutingSlipCompleted> DeleteProfileSuccess { get; private set; }
    public Event<RoutingSlipFaulted> DeleteProfileFailed { get; private set; }

    private void InitOrchestratorInstance(BehaviorContext<SagaOrchestratorInstance> context,
        IEndpointNameFormatter endpointNameFormatter)
    {
        context.Saga.OrchestratorInstanceAddress =
            new Uri($"exchange:{endpointNameFormatter.Saga<SagaOrchestratorInstance>()}");
    }

    private async Task CourierStart(BehaviorContext<SagaOrchestratorInstance,
        DeleteProfileRequest> context
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

    private RoutingSlip CreateRoutingSlip(DeleteProfileRequest request,
        Uri targetSagaAddress,
        Guid trackingNumber)
    {
        var mapper = _serviceProvider.GetRequiredService<IMapper>();

        var builder = new RoutingSlipBuilder(trackingNumber);

        AddUserServiceDeleteProfileCommandActivity(builder, request);

        AddOrderServiceDeleteProfileCommandActivity(builder, mapper, request);

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

    private void AddUserServiceDeleteProfileCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        DeleteProfileRequest request)
    {
        DeleteProfileCommand deleteProfileCommandToUserService = request;

        routingSlipBuilder.AddActivity(
            UserServiceDeleteProfileCommandActivityName,
            DeleteProfileCommandActivityContracts.ExecuteEndpoint.Uri,
            deleteProfileCommandToUserService);
    }

    private void AddOrderServiceDeleteProfileCommandActivity(
        RoutingSlipBuilder routingSlipBuilder,
        IMapper mapper,
        DeleteProfileRequest request)
    {
        var deleteProfileCommandToOrderService = mapper
            .Map<Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete.DeleteProfileCommand>(request);

        routingSlipBuilder.AddActivity(
            OrderServiceDeleteProfileCommandActivityName,
            Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete.DeleteProfileCommandActivityContracts
                .ExecuteEndpoint.Uri,
            deleteProfileCommandToOrderService);
    }
}
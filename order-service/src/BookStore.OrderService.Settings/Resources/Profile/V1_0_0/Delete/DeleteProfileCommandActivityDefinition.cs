using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Profile.V1_0_0.Delete;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.WebEntryPoint.Profile.V1_0_0.Delete.MassTransitCourierActivities;
using MassTransit;

namespace BookStore.OrderService.Settings.Resources.Profile.V1_0_0.Delete;

public class DeleteProfileCommandActivityDefinition :
    ActivityDefinition<DeleteProfileCommandActivity, DeleteProfileCommand, BL.ResourceEntities.Profile>
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteProfileCommandActivityDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ExecuteEndpointName = DeleteProfileCommandActivityContracts.ExecuteEndpoint.Name;
    }

    protected override void ConfigureExecuteActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        IExecuteActivityConfigurator<DeleteProfileCommandActivity, DeleteProfileCommand>
            executeActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }

    protected override void ConfigureCompensateActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        ICompensateActivityConfigurator<DeleteProfileCommandActivity, BL.ResourceEntities.Profile>
            compensateActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }
}
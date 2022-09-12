using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;
using MassTransit;

namespace BookStore.UserService.Settings.Resources.Book.V1_0_0.Delete;

public class DeleteBookCommandActivityDefinition :
    ActivityDefinition<DeleteBookCommandActivity, DeleteBookCommand, BL.ResourceEntities.Book>
{
    private readonly IServiceProvider _serviceProvider;

    public DeleteBookCommandActivityDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ExecuteEndpointName = DeleteBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
    protected override void ConfigureExecuteActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        IExecuteActivityConfigurator<DeleteBookCommandActivity, DeleteBookCommand>
            executeActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }

    protected override void ConfigureCompensateActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        ICompensateActivityConfigurator<DeleteBookCommandActivity, BL.ResourceEntities.Book>
            compensateActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }
}
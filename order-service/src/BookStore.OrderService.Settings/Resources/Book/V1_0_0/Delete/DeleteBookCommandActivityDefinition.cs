using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.OrderService.Contracts.Book.V1_0_0.Delete;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;
using MassTransit;

namespace BookStore.OrderService.Settings.Resources.Book.V1_0_0.Delete;

public class DeleteBookCommandActivityDefinition :
    ActivityDefinition<DeleteBookCommandActivity, DeleteBookCommand, DeleteBookCompensateCommand>
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
        ICompensateActivityConfigurator<DeleteBookCommandActivity, DeleteBookCompensateCommand>
            compensateActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }
}
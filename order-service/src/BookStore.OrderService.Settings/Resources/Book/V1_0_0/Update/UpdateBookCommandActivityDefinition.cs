using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update;
using BookStore.OrderService.Contracts.Book.V1_0_0.Update;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;
using MassTransit;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.OrderService.Settings.Resources.Book.V1_0_0.Update;

public class UpdateBookCommandActivityDefinition : ActivityDefinition<
    UpdateBookCommandActivity, UpdateBookCommand, UpdateBookCompensateCommand>
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateBookCommandActivityDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ExecuteEndpointName = UpdateBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
    protected override void ConfigureExecuteActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        IExecuteActivityConfigurator<UpdateBookCommandActivity, UpdateBookCommand>
            executeActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }

    protected override void ConfigureCompensateActivity(
        IReceiveEndpointConfigurator endpointConfigurator,
        ICompensateActivityConfigurator<UpdateBookCommandActivity, UpdateBookCompensateCommand>
            compensateActivityConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_serviceProvider);
    }
}
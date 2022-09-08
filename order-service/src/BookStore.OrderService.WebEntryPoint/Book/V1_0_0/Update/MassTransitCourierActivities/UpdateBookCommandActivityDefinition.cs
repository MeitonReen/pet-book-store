using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update;
using BookStore.OrderService.Contracts.Book.V1_0_0.Update;
using MassTransit;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;

public class UpdateBookCommandActivityDefinition : ActivityDefinition<
    UpdateBookCommandActivity, UpdateBookCommand, UpdateBookCompensateCommand>
{
    public UpdateBookCommandActivityDefinition()
    {
        ExecuteEndpointName = UpdateBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
}
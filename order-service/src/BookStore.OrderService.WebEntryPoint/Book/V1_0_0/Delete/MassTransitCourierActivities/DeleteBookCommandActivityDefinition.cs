using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.OrderService.Contracts.Book.V1_0_0.Delete;
using MassTransit;

namespace BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteBookCommandActivityDefinition :
    ActivityDefinition<DeleteBookCommandActivity, DeleteBookCommand, DeleteBookCompensateCommand>
{
    public DeleteBookCommandActivityDefinition()
    {
        ExecuteEndpointName = DeleteBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
}
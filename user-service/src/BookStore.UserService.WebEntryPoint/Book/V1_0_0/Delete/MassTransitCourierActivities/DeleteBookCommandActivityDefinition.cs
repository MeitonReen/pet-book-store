using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete;
using MassTransit;

namespace BookStore.UserService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteBookCommandActivityDefinition :
    ActivityDefinition<DeleteBookCommandActivity, DeleteBookCommand, BL.ResourceEntities.Book>
{
    public DeleteBookCommandActivityDefinition()
    {
        ExecuteEndpointName = DeleteBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
}
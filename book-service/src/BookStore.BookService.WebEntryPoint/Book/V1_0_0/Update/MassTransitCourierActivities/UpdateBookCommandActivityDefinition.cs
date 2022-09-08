using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using MassTransit;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;

public class UpdateBookCommandActivityDefinition :
    ActivityDefinition<UpdateBookCommandActivity, UpdateBookCommand, UpdateBookCompensateCommand>
{
    public UpdateBookCommandActivityDefinition()
    {
        ExecuteEndpointName = UpdateBookCommandActivityContracts.ExecuteEndpoint.Name;
    }
}
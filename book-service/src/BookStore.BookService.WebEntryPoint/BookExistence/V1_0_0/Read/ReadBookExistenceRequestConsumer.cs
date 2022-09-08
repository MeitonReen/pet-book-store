using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.BookExistence.V1_0_0.Read;
using MassTransit;

namespace BookStore.BookService.WebEntryPoint.BookExistence.V1_0_0.Read;

public class ReadBookExistenceRequestConsumer :
    IConsumer<ReadBookExistenceRequest>
{
    private readonly IBaseResourceExistence<BL.ResourceEntities.Book> _presenceBookResource;

    public ReadBookExistenceRequestConsumer(
        IBaseResourceExistence<BL.ResourceEntities.Book> presenceBookResource
    )
    {
        _presenceBookResource = presenceBookResource;
    }

    public async Task Consume(ConsumeContext<ReadBookExistenceRequest> context)
    {
        var configuredResource = _presenceBookResource.ReadSettings(book =>
            book.BookId = context.Message.BookId);

        bool targetResourceIsPresentInResourceCollection;
        try
        {
            targetResourceIsPresentInResourceCollection = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResourceIsPresentInResourceCollection = await configuredResource.ReadAsync();
        }

        if (targetResourceIsPresentInResourceCollection)
        {
            await context.RespondAsync<Found>(new { });
        }
        else
        {
            await context.RespondAsync<NotFound>(new { });
        }
    }
}
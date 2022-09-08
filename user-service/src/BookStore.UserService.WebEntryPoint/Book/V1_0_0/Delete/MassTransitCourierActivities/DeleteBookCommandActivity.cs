using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.Book.V1_0_0.Delete;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteBookCommandActivity : IActivity<DeleteBookCommand, BL.ResourceEntities.Book>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteBookCommandActivity(
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource
    )
    {
        _resourcesCommitter = resourcesCommitter;
        _bookBaseResource = bookBaseResource;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeleteBookCommand> context)
    {
        var oldEntityState = await _bookBaseResource
            .ReadSettings(settings => settings
                .Where(book => book.BookId == context.Arguments.BookId)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityState == default) return context.Completed();

        _bookBaseResource.Delete(book => book.BookId = context.Arguments.BookId);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Completed(oldEntityState);
    }

    public async Task<CompensationResult> Compensate(CompensateContext<BL.ResourceEntities.Book> context)
    {
        var oldEntityState = context.Log;

        _bookBaseResource.Create(oldEntityState);
        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Compensated();
    }
}
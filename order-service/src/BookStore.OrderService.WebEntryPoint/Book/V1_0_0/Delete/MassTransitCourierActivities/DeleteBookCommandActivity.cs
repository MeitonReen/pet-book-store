using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.OrderService.Contracts.Book.V1_0_0.Delete;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteBookCommandActivity : IActivity<DeleteBookCommand, DeleteBookCompensateCommand>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IDeleteBookResource _deleteBookResource;
    private readonly IConfigurationProvider _mapperConfigurationProvider;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteBookCommandActivity(
        IDeleteBookResource deleteBookResource,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource,
        IConfigurationProvider mapperConfigurationProvider
    )
    {
        _deleteBookResource = deleteBookResource;
        _resourcesCommitter = resourcesCommitter;
        _bookBaseResource = bookBaseResource;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeleteBookCommand> context)
    {
        var oldEntityState = await _bookBaseResource
            .ReadSettings(settings => settings
                .Where(book => book.BookId == context.Arguments.BookId)
                .ProjectTo<DeleteBookCompensateCommand>(_mapperConfigurationProvider)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityState == default) return context.Completed();

        await _deleteBookResource.Delete(context.Arguments.BookId);

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

    public async Task<CompensationResult> Compensate(CompensateContext<DeleteBookCompensateCommand> context)
    {
        _bookBaseResource.Update(book => book.BookId = context.Log.BookId,
            updateSets => updateSets.Deleted = false);

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
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;

public class UpdateBookCommandActivity : IActivity<UpdateBookCommand, UpdateBookCompensateCommand>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IMapper _mapper;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUpdateBookResource _updateBookResource;

    public UpdateBookCommandActivity(
        IUpdateBookResource updateBookResource,
        IMapper mapper,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource
    )
    {
        _updateBookResource = updateBookResource;
        _mapper = mapper;
        _resourcesCommitter = resourcesCommitter;
        _bookBaseResource = bookBaseResource;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<UpdateBookCommand> context)
    {
        var oldEntityStateAsBookUpdateRequest = await _bookBaseResource
            .ReadSettings(settings => settings
                .Where(book => book.BookId == context.Arguments.BookId)
                .ProjectTo<UpdateBookCompensateCommand>(_mapper.ConfigurationProvider)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityStateAsBookUpdateRequest == default)
            throw new InvalidOperationException("Requested resource not found");

        await _updateBookResource.Update(context.Arguments);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return context.Completed(oldEntityStateAsBookUpdateRequest);
    }

    public async Task<CompensationResult> Compensate(CompensateContext<UpdateBookCompensateCommand> context)
    {
        var oldEntityState = context.Log;

        await _updateBookResource.Update(oldEntityState);

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
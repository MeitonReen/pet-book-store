using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.OrderService.Contracts.Book.V1_0_0.Update;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using UpdateBookCommand = BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update.UpdateBookCommand;

namespace BookStore.OrderService.WebEntryPoint.Book.V1_0_0.Update.MassTransitCourierActivities;

public class UpdateBookCommandActivity : IActivity<UpdateBookCommand, UpdateBookCompensateCommand>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IMapper _mapper;
    private readonly IResourcesCommitter _resourcesCommitter;

    public UpdateBookCommandActivity(
        IMapper mapper,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource
    )
    {
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

        if (oldEntityStateAsBookUpdateRequest == default) return context.Completed();

        _bookBaseResource.Update(context.Arguments.ToEntity());

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

        _bookBaseResource.Update(oldEntityState.ToEntity());

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

public static class BookUpdatedMessageExtensions
{
    public static BL.ResourceEntities.Book ToEntity(this UpdateBookCommand message)
        => new()
        {
            BookId = message.BookId,
            Name = message.Name,
            PublicationDate = message.PublicationDate,
            Price = message.Price
        };
}
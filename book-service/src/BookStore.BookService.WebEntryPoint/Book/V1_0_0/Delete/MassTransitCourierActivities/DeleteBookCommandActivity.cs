using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace BookStore.BookService.WebEntryPoint.Book.V1_0_0.Delete.MassTransitCourierActivities;

public class DeleteBookCommandActivity : IActivity<DeleteBookCommand, BL.ResourceEntities.Book>
{
    private readonly IBaseResource<BL.ResourceEntities.Book> _bookBaseResource;
    private readonly IDeleteBookResource _deleteBookResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteBookCommandActivity(
        IDeleteBookResource deleteBookResource,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<BL.ResourceEntities.Book> bookBaseResource
    )
    {
        _deleteBookResource = deleteBookResource;
        _resourcesCommitter = resourcesCommitter;
        _bookBaseResource = bookBaseResource;
    }

    public async Task<ExecutionResult> Execute(ExecuteContext<DeleteBookCommand> context)
    {
        var oldEntityState = await _bookBaseResource
            .ReadSettings(settings => settings
                .Where(book => book.BookId == context.Arguments.BookId)
                .Include(book => book.Authors)
                .Include(book => book.Categories)
                .AsNoTracking())
            .ReadAsync();

        if (oldEntityState == default)
            throw new InvalidOperationException("Requested resource not found");

        ClearCircularReferences(oldEntityState);

        await _deleteBookResource.Delete(context.Arguments);

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

        CompensateCompositeEntityAfterDelete(oldEntityState);

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

    private void CompensateCompositeEntityAfterDelete(BL.ResourceEntities.Book entity)
    {
        var authors = entity.Authors;
        var bookCategories = entity.Categories;

        entity.Authors = new List<BL.ResourceEntities.Author>();
        entity.Categories = new List<BL.ResourceEntities.BookCategory>();

        _bookBaseResource
            .Create(entity)
            .CreateReferences(author => author.Authors, authors)
            .CreateReferences(author => author.Categories, bookCategories);
    }

    private void ClearCircularReferences(BL.ResourceEntities.Book book)
    {
        book.Authors = book.Authors
            .Select(author =>
            {
                author.Books = new List<BL.ResourceEntities.Book>();
                return author;
            })
            .ToArray();

        book.Categories = book.Categories
            .Select(category =>
            {
                category.Books = new List<BL.ResourceEntities.Book>();
                return category;
            })
            .ToArray();
    }
}
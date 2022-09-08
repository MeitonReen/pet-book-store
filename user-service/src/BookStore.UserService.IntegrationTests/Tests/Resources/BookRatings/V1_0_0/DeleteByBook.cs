using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.BookRatings.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.BookRatings.V1_0_0;

public class DeleteByBook : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookRating> _bookRatingResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteByBook(
        IBaseResourceCollection<BL.ResourceEntities.BookRating> bookRatingResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IResourcesCommitter resourcesCommitter
    )
    {
        _bookRatingResourceCollection = bookRatingResourceCollection;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _resourcesCommitter = resourcesCommitter;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task DeleteByBook_ValidRequest_BookRatingsByBookIsDeleted(
        DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;

        var targetBookRatings = await _bookRatingResourceCollection
            .ReadSettings(sets => sets
                .Where(bookRating => bookRating.Book != default
                                     && bookRating.Book.BookId == request.BookId)
                .Select(bookRating => new BL.ResourceEntities.BookRating
                    {RatingId = bookRating.RatingId})
                .AsNoTracking())
            .ReadAsync();

        _bookRatingResourceCollection.Delete(targetBookRatings);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        //Assert
        Assert.True(true);
    }

    [Theory, AutoDataBookStore]
    public async Task DeleteByBook_InvalidRequest_NonRepeatCommitException(
        Guid inputBookRatingId)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var targetBookRatings = new[] {new BL.ResourceEntities.BookRating {RatingId = inputBookRatingId}};

        _bookRatingResourceCollection.Delete(targetBookRatings);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }
        //Assert
        catch (Exception)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }
}
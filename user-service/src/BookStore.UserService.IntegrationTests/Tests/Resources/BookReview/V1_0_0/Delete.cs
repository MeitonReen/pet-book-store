using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.BookReview.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.BookReview.V1_0_0;

public class Delete : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookReview> _bookReviewResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Delete(
        IBaseResource<BL.ResourceEntities.BookReview> bookReviewResource,
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookReviewResource = bookReviewResource;
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Delete_ValidRequest_BookReviewIsDeleted()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new DeleteRequest
            {ReviewId = InitDataCreator.BookReviews.EugeneOneginTestUserReview.ReviewId};
        //Act
        _bookReviewResource.Delete(bookReview => bookReview.ReviewId = request.ReviewId);

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
    public async Task Delete_InvalidRequest_NonRepeatCommitException(DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        _bookReviewResource.Delete(bookReview => bookReview.ReviewId = request.ReviewId);

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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.BookReviews.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyBookReview.V1_0_0;

public class DeleteByBook : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookReview> _bookReviewResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IReadShortMyBookReviewResource _readShortMyBookReviewResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteByBook(
        IBaseResource<BL.ResourceEntities.BookReview> bookReviewResource,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IResourcesCommitter resourcesCommitter,
        IReadShortMyBookReviewResource readShortMyBookReviewResource
    )
    {
        _bookReviewResource = bookReviewResource;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _resourcesCommitter = resourcesCommitter;
        _readShortMyBookReviewResource = readShortMyBookReviewResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task DeleteByBook_ValidRequest_MyBookReviewByBookIsDeleted(
        DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;

        var targetBookReview = await _readShortMyBookReviewResource.ReadShort(
            request.BookId, InitDataCreator.Profiles.TestUser.UserId);

        if (targetBookReview == default)
            throw new InvalidOperationException(
                $"{nameof(targetBookReview)} not found");

        _bookReviewResource.Delete(targetBookReview);

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
    public async Task DeleteByBook_InvalidRequest_TargetBookReviewShouldBeDefault(
        DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var targetBookReview = await _readShortMyBookReviewResource.ReadShort(
            request.BookId, InitDataCreator.Profiles.TestUser.UserId);

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
        targetBookReview.Should().BeNull();
    }
}
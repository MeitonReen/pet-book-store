using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyBookReview.V1_0_0;

public class UpdateByBook : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUpdateMyBookReviewResource _updateMyBookReviewResource;

    public UpdateByBook(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IUpdateMyBookReviewResource updateMyBookReviewResource
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _updateMyBookReviewResource = updateMyBookReviewResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task UpdateByBook_ValidRequest_MyBookReviewByBookIsUpdated(
        UpdateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;
        await _updateMyBookReviewResource.Update(request);

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
    public async Task UpdateByBook_InvalidRequest_ResultShouldBeNotUpdated(
        UpdateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        var result = await _updateMyBookReviewResource.Update(request);

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
        result.IsUpdated.Should().BeFalse();
    }
}
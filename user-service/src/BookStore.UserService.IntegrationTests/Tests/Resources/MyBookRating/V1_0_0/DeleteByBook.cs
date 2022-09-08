using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.BookRatings.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyBookRating.V1_0_0;

public class DeleteByBook : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IReadShortMyBookRatingResource _readShortMyBookRatingResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public DeleteByBook(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IResourcesCommitter resourcesCommitter,
        IReadShortMyBookRatingResource readShortMyBookRatingResource
    )
    {
        _bookRatingResource = bookRatingResource;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _resourcesCommitter = resourcesCommitter;
        _readShortMyBookRatingResource = readShortMyBookRatingResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task DeleteByBook_ValidRequest_MyBookRatingByBookIsDeleted(
        DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;

        var targetBookRating = await _readShortMyBookRatingResource.ReadShort(
            request.BookId, InitDataCreator.Profiles.TestUser.UserId);

        if (targetBookRating == default)
            throw new InvalidOperationException(
                $"{nameof(targetBookRating)} not found");

        _bookRatingResource.Delete(targetBookRating);

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
    public async Task DeleteByBook_InvalidRequest_TargetBookRatingShouldBeDefault(
        DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var targetBookRating = await _readShortMyBookRatingResource.ReadShort(
            request.BookId, InitDataCreator.Profiles.TestUser.UserId);

        //Act
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
        targetBookRating.Should().BeNull();
    }
}
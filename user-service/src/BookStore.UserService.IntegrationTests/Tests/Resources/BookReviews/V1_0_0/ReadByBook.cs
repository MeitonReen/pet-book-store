using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.BookReviews.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.BookReviews.V1_0_0;

public class ReadByBook : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookReview> _bookReviewResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public ReadByBook(
        IBaseResourceCollection<BL.ResourceEntities.BookReview> bookReviewResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookReviewResourceCollection = bookReviewResourceCollection;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task ReadByBook_ValidRequest_BookReviewsByBookRead(ReadPartRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;

        var configuredResourceCollection = _bookReviewResourceCollection
            .ReadSettings(sets => sets
                .Where(bookReview => bookReview.Book != default
                                     && bookReview.Book.BookId == request.BookId)
                .AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.BookReview> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }

        //Assert
        targetResource.Should().BeEquivalentTo(new[]
        {
            InitDataCreator.BookReviews.EugeneOneginTestUserReview
        }, sets => sets.Excluding(bookReview => bookReview.DateTimeSet));
    }

    [Theory, AutoDataBookStore]
    public async Task ReadByBook_InvalidRequest_ResultShouldBeDefault(ReadPartRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResourceCollection = _bookReviewResourceCollection
            .ReadSettings(sets => sets
                .Where(bookReview => bookReview.Book != default
                                     && bookReview.Book.BookId == request.BookId)
                .AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.BookReview> targetResult;
        try
        {
            targetResult = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResourceCollection.ReadAsync();
        }

        //Assert
        targetResult.Should().BeEmpty();
    }
}
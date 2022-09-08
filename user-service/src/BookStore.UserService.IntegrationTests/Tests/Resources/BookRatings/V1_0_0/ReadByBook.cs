using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.BookRatings.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.BookRatings.V1_0_0;

public class ReadByBook : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookRating> _bookRatingResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public ReadByBook(
        IBaseResourceCollection<BL.ResourceEntities.BookRating> bookRatingResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookRatingResourceCollection = bookRatingResourceCollection;

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
    public async Task ReadByBook_ValidRequest_BookRatingsByBookRead(ReadPartRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;

        var configuredResourceCollection = _bookRatingResourceCollection
            .ReadSettings(sets => sets
                .Where(bookRating => bookRating.Book != default
                                     && bookRating.Book.BookId == request.BookId)
                .AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.BookRating> targetResource;
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
            InitDataCreator.BookRatings.EugeneOneginTestUserRating
        }, sets => sets.Excluding(bookRating => bookRating.DateTimeSet));
    }

    [Theory, AutoDataBookStore]
    public async Task ReadByBook_InvalidRequest_ResultShouldBeDefault(ReadPartRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResourceCollection = _bookRatingResourceCollection
            .ReadSettings(sets => sets
                .Where(bookRating => bookRating.Book != default
                                     && bookRating.Book.BookId == request.BookId)
                .AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.BookRating> targetResult;
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
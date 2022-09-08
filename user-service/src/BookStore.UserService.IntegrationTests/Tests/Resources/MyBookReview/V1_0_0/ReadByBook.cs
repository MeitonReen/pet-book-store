using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyBookReview.V1_0_0;

public class ReadByBook : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookReview> _bookReviewResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;

    public ReadByBook(
        IBaseResource<BL.ResourceEntities.BookReview> bookReviewResource,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IMapper mapper
    )
    {
        _bookReviewResource = bookReviewResource;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _mapper = mapper;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ReadByBook_ValidRequest_MyBookReviewByBookIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new ReadRequest {BookId = InitDataCreator.Books.EugeneOnegin.BookId};

        var configuredResource = _bookReviewResource
            .ReadSettings(sets => sets
                .Where(bookReview =>
                    bookReview.Book != default
                    && bookReview.Book.BookId == request.BookId
                    && bookReview.Profile.UserId == InitDataCreator.Profiles.TestUser.UserId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
            );

        var eugeneOneginTestUserRating = InitDataCreator.BookReviews.EugeneOneginTestUserReview;
        eugeneOneginTestUserRating.Book = InitDataCreator.Books.EugeneOnegin;
        eugeneOneginTestUserRating.Profile = InitDataCreator.Profiles.TestUser;

        var verificationResponse = _mapper.Map<ReadResponse>(eugeneOneginTestUserRating);
        // Act
        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        //Assert
        targetResult.Should().BeEquivalentTo(verificationResponse,
            sets => sets.Excluding(verResponse => verResponse.DateTimeSet));
    }

    [Theory, AutoDataBookStore]
    public async Task ReadByBook_InvalidRequest_ResultShouldBeDefault(ReadRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _bookReviewResource
            .ReadSettings(sets => sets
                .Where(bookReview =>
                    bookReview.Book != default
                    && bookReview.Book.BookId == request.BookId
                    && bookReview.Profile.UserId == InitDataCreator.Profiles.TestUser.UserId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
            );

        // Act
        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        //Assert
        targetResult.Should().BeNull();
    }
}
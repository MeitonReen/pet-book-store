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
using BookStore.UserService.Contracts.BookRating.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.BookRating.V1_0_0;

public class Read : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;

    public Read(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IMapper mapper
    )
    {
        _bookRatingResource = bookRatingResource;

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
    public async Task Read_ValidRequest_BookRatingRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _bookRatingResource
            .ReadSettings(settings => settings
                .Where(bookRating =>
                    bookRating.RatingId == InitDataCreator.BookRatings.EugeneOneginTestUserRating.RatingId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

        var eugeneOneginTestUserRating = InitDataCreator.BookRatings.EugeneOneginTestUserRating;
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
        targetResult.Should().BeEquivalentTo(verificationResponse, sets => sets.Excluding(
            verResponse => verResponse.DateTimeSet));
    }

    [Theory, AutoDataBookStore]
    public async Task Read_InvalidRequest_ResultShouldBeDefault(ReadRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _bookRatingResource
            .ReadSettings(settings => settings
                .Where(bookRating => bookRating.RatingId == request.RatingId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());
        ;
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

        targetResult.Should().BeNull();
    }
}
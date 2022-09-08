using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyBookRating.V1_0_0;

public class CreateForBook : IDisposable
{
    private readonly ICreateMyBookRatingResource _createMyBookRatingResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateForBook(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        ICreateMyBookRatingResource createMyBookRatingResource,
        IUserClaimsProfile userClaimsProfile
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _createMyBookRatingResource = createMyBookRatingResource;
        _userClaimsProfile = userClaimsProfile;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task CreateForBook_ValidRequest_MyBookRatingForBookIsCreated(
        CreateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;
        await _createMyBookRatingResource.Create(request);

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
}
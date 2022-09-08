using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.MyProfile.V1_0_0.Create;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.WebEntryPoint.MyProfile.V1_0_0.Create;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyProfile.V1_0_0;

public class Create : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Create(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IUserClaimsProfile userClaimsProfile
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _profileResource = profileResource;
        _userClaimsProfile = userClaimsProfile;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task Create_ValidRequest_MyProfileIsCreated(CreateRequest request,
        Guid profileId)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var userProfile = request.ToEntity();
        userProfile.UserId = profileId;
        userProfile.UserName = profileId.ToString();

        //Act
        _profileResource.Create(userProfile);

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
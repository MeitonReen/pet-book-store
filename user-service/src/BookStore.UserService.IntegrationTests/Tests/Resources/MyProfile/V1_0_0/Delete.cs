using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.Profile.V1_0_0.Delete;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.WebEntryPoint.MyProfile.V1_0_0.Create;
using MassTransit;
using MassTransit.Testing;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyProfile.V1_0_0;

public class Delete : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly ITestHarness _testHarness;
    private readonly IPublishEndpoint _transactionOutboxPublishEndpoint;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Delete(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        ITestHarness testHarness,
        IMapper mapper,
        IPublishEndpoint transactionOutboxPublishEndpoint,
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IUserClaimsProfile userClaimsProfile
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _testHarness = testHarness;
        _mapper = mapper;
        _transactionOutboxPublishEndpoint = transactionOutboxPublishEndpoint;
        _profileResource = profileResource;
        _userClaimsProfile = userClaimsProfile;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Delete_ValidRequest_ProfileIsDeleted()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var userProfile = _userClaimsProfile.FillEntity(new BL.ResourceEntities.Profile());

        //Act
        _profileResource.Delete(profile => profile.UserId = userProfile.UserId);

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
        _profileResource.Delete(profile => profile.UserId = request.UserId);

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
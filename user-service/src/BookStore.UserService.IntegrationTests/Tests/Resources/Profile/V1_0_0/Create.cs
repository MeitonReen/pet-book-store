using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.UserService.Contracts.Profile.V1_0_0.Create;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using BookStore.UserService.WebEntryPoint.Profile.V1_0_0.Create;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.Profile.V1_0_0;

public class Create : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Create(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IBaseResource<BL.ResourceEntities.Profile> profileResource
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _profileResource = profileResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    public async Task Create_ValidRequest_ProfileIsCreated(CreateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.UserId = new Guid("8c1d9f41-db3b-4652-9d9d-00bf853c33c9");
        request.UserName = request.UserId.ToString();

        //Act
        _profileResource.Create(request.ToEntity());

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
    public async Task Create_InvalidRequest_NonRepeatCommitException(CreateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.UserId = InitDataCreator.Profiles.TestUser.UserId;
        //Act
        _profileResource.Create(request.ToEntity());

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
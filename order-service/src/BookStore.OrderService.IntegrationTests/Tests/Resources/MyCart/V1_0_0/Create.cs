using System;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.NonAttachedExtensions;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Abstractions;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.OrderService.IntegrationTests.Tests.Resources.MyCart.V1_0_0;

public class Create : IDisposable
{
    private readonly ICreateMyCartResource _createMyCartResource;
    private readonly IDatabaseInit _databaseInit;
    private readonly BaseDbContext _dbContext;
    private readonly IBaseResourceExistence<Profile> _profileExistence;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Create(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IDatabaseInit databaseInit,
        ICreateMyCartResource createMyCartResource,
        IBaseResourceExistence<Profile> profileExistence
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInit = databaseInit;
        _createMyCartResource = createMyCartResource;
        _profileExistence = profileExistence;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Create_ValidUserClaimsProfile_MyCartIsCreated()
    {
        //Arrange
        await DatabaseTestHelper
            .Initialize(_databaseInit, _dbContext, dbContextInner => dbContextInner
                .Set<Profile>()
                .AddIfNotContain(InitDataCreator.Profiles.TestUser));

        var s = await _profileExistence
            .ReadSettings(profile => profile.UserId = InitDataCreator.Profiles.TestUser.UserId)
            .ReadAsync();
        ;
        //Act
        var result = await _createMyCartResource.Create();

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
        result.IsCreated.Should().BeTrue();
    }
}
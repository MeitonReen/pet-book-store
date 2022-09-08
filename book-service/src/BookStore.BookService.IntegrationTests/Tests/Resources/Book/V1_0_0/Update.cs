using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0.Update;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Book.V1_0_0;

public class Update : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUpdateBookResource _updateBookResource;

    public Update(
        IUpdateBookResource updateBookResource,
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _updateBookResource = updateBookResource;
        _resourcesCommitter = resourcesCommitter;

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
    public async Task Update_ValidRequest_BookIsUpdated(UpdateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        request.BookId = InitDataCreator.Books.EugeneOnegin.BookId;
        //Act
        var targetResult = await _updateBookResource.Update(request);

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
        targetResult.IsUpdated.Should().BeTrue();
    }

    [Theory, AutoDataBookStore]
    public async Task Update_InvalidRequest_NonRepeatCommitException(UpdateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        await _updateBookResource.Update(request);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception)
        {
            Assert.True(true);
            return;
        }

        Assert.False(true);
    }
}
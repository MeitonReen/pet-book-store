using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.BookService.BL.Resources.Book.V1_0_0.Update.Abstractions;
using BookStore.BookService.Contracts.Book.V1_0_0.Delete;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Book.V1_0_0;

public class Delete : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IDeleteBookResource _deleteBookResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Delete(
        IUpdateBookResource updateBookResource,
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IDeleteBookResource deleteBookResource
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _deleteBookResource = deleteBookResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Delete_ValidRequest_BookIsDeleted()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new DeleteRequest {BookId = InitDataCreator.Books.EugeneOnegin.BookId};

        //Act
        var targetResult = await _deleteBookResource.Delete(request);

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
        targetResult.IsDeleted.Should().BeTrue();
    }

    [Theory, AutoDataBookStore]
    public async Task Delete_InvalidRequest_NonRepeatCommitException(DeleteRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        await _deleteBookResource.Delete(request);

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

        Assert.True(false);
    }
}
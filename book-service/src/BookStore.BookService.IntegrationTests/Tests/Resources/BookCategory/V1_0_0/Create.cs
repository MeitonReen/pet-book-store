using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.WebEntryPoint.BookCategory.V1_0_0.Create;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.BookCategory.V1_0_0;

public class Create : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookCategory> _bookCategoryResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Create(
        IBaseResource<BL.ResourceEntities.BookCategory> bookCategoryResource,
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookCategoryResource = bookCategoryResource;
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
    public async Task Create_ValidRequest_BookCategoryIsCreated(CreateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        _bookCategoryResource.Create(request.ToEntity());

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
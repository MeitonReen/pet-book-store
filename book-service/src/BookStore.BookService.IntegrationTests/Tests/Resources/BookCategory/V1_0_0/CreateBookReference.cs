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
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.BookCategory.V1_0_0;

public class CreateBookReference : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.BookCategory> _bookCategoryResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    private readonly IResourcesCommitter _resourcesCommitter;

    // private readonly CreateRequest request
    public CreateBookReference(
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

    [Fact]
    // [MemberData(nameof(Data))]
    public async Task CreateBookReference_ValidRequest_ReferenceIsCreated()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new CreateBookReferenceRequest
        {
            BookId = InitDataCreator.Books.EugeneOnegin.BookId,
            CategoryId = InitDataCreator.BookCategories.Poetry.CategoryId
        };
        //Act
        _bookCategoryResource.CreateReference(
            bookCategory => bookCategory.CategoryId = request.CategoryId,
            path => path.Books,
            book => book.BookId = request.BookId);

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
    public async Task CreateBookReference_InvalidRequest_NonRepeatCommitException(
        CreateBookReferenceRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        _bookCategoryResource.CreateReference(
            bookCategory => bookCategory.CategoryId = request.CategoryId,
            path => path.Books,
            book => book.BookId = request.BookId);

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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Abstractions;
using BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Xunit;

namespace BookStore.OrderService.IntegrationTests.Tests.Resources.MyCart.V1_0_0;

public class CreateBook : IDisposable
{
    private readonly ICreateBookInMyCartResource _createBookInMyCartResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;

    public CreateBook(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        ICreateBookInMyCartResource createBookInMyCartResource
    )
    {
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _createBookInMyCartResource = createBookInMyCartResource;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task CreateBook_CreatingBookIsNotExistsInMyCart_BookInMyCartIsCreated()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new CreateBookRequest {BookId = InitDataCreator.Books.EugeneOnegin.BookId};
        //Act
        var result = await _createBookInMyCartResource.CreateBook(request);

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

    [Fact]
    public async Task CreateBook_CreatingBookIsExistsInMyCart_BookInMyCartIsCreated()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new CreateBookRequest {BookId = InitDataCreator.Books.TheGambler.BookId};
        //Act
        var result = await _createBookInMyCartResource.CreateBook(request);

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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.BookCategories.V1_0_0;

public class Read : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookCategory> _bookCategoryResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public Read(
        IBaseResourceCollection<BL.ResourceEntities.BookCategory> bookCategoryResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookCategoryResourceCollection = bookCategoryResourceCollection;

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
    public async Task Read_ReadResourceCollection_BookCategoriesIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResourceCollection = _bookCategoryResourceCollection
            .ReadSettings(sets => sets.AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.BookCategory> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }

        //Assert
        targetResource.Should().BeEquivalentTo(new[]
        {
            InitDataCreator.BookCategories.Classic,
            InitDataCreator.BookCategories.Poetry
        });
    }
}
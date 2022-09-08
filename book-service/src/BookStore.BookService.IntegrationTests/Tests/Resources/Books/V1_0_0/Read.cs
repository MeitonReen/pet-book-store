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

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Books.V1_0_0;

public class Read : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.Book> _bookResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public Read(
        IBaseResourceCollection<BL.ResourceEntities.Book> bookResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _bookResourceCollection = bookResourceCollection;

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
    public async Task Read_ReadResourceCollection_BooksIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResourceCollection = _bookResourceCollection
            .ReadSettings(sets => sets.AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.Book> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }

        ;
        //Assert
        targetResource.Should().BeEquivalentTo(new[]
        {
            InitDataCreator.Books.EugeneOnegin,
            InitDataCreator.Books.TheGambler,
            InitDataCreator.Books.CrimeAndPunishment
        });
    }
}
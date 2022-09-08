using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.BookService.Contracts.FilterableBooks.V1_0_0.Read;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using BookStore.BookService.WebEntryPoint.FilterableBooks.V1_0_0.Read.Extensions;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Books.V1_0_0;

public class ReadWithFiltersApplied : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.Book> _bookResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public ReadWithFiltersApplied(
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
    public async Task Read_ValidRequest_BooksIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new ReadPartRequest
        {
            CategoryName = new string(InitDataCreator.BookCategories.Classic.Name.Take(4).ToArray()),
            AuthorLastName = new string(InitDataCreator.Authors.Pushkin.LastName.Take(4).ToArray()),
            OrderPrice = SortOrder.Ascending
        };

        var configuredResourceCollection = _bookResourceCollection
            .ReadSettings(sets => sets
                .ApplyTargetFilters(request)
                .AsNoTracking());

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

        //Assert
        targetResource.Should().BeEquivalentTo(new[]
        {
            InitDataCreator.Books.EugeneOnegin
        });
    }
}
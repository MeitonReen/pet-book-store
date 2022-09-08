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

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Authors.V1_0_0;

public class Read : IDisposable
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.Author> _authorResourceCollection;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    public Read(
        IBaseResourceCollection<BL.ResourceEntities.Author> authorResourceCollection,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _authorResourceCollection = authorResourceCollection;

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
    public async Task Read_ReadResourceCollection_AuthorsIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResourceCollection = _authorResourceCollection
            .ReadSettings(sets => sets.AsNoTracking());

        //Act
        IEnumerable<BL.ResourceEntities.Author> targetResource;
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
            InitDataCreator.Authors.Chekhov,
            InitDataCreator.Authors.Dostoevskiy,
            InitDataCreator.Authors.Pushkin
        });
    }
}
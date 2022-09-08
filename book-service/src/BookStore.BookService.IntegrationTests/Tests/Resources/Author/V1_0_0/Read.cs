using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.BookService.Contracts.Author.V1_0_0.Read;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Author.V1_0_0;

public class Read : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.Author> _authorResource;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;

    public Read(
        IBaseResource<BL.ResourceEntities.Author> authorResource,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IMapper mapper
    )
    {
        _authorResource = authorResource;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _mapper = mapper;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Read_ValidRequest_AuthorIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _authorResource
            .ReadSettings(settings => settings
                .Where(author => author.AuthorId == InitDataCreator.Authors.Chekhov.AuthorId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

        var verificationResponse = _mapper.Map<ReadResponse>(InitDataCreator.Authors.Chekhov);

        // Act
        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        //Assert
        targetResult.Should().BeEquivalentTo(verificationResponse);
        Assert.True(true);
    }

    [Theory, AutoDataBookStore]
    public async Task Read_InvalidRequest_ResultShouldBeDefault(ReadRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _authorResource
            .ReadSettings(settings => settings
                .Where(author => author.AuthorId == request.AuthorId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());
        ;
        // Act
        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        targetResult.Should().BeNull();
    }
}
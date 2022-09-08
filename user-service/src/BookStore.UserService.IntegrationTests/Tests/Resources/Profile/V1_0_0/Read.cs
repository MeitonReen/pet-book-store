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
using BookStore.UserService.Contracts.Profile.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.Profile.V1_0_0;

public class Read : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;

    public Read(
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _profileResource = profileResource;
        _mapper = mapper;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Read_ValidRequest_ProfileIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var request = new ReadRequest {UserId = InitDataCreator.Profiles.TestUser.UserId};

        var verificationResponse = _mapper.Map<ReadResponse>(InitDataCreator.Profiles.TestUser);

        var configuredResource = _profileResource
            .ReadSettings(settings => settings
                .Where(profile => profile.UserId == request.UserId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());
        //Act
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
    }

    [Theory, AutoDataBookStore]
    public async Task Read_InvalidRequest_ResultShouldBeDefault(ReadRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var configuredResource = _profileResource
            .ReadSettings(settings => settings
                .Where(profile => profile.UserId == request.UserId)
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

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
        targetResult.Should().BeNull();
    }
}
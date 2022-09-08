using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.UserService.Contracts.Profile.V1_0_0.Read;
using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.UserService.IntegrationTests.Tests.Resources.MyProfile.V1_0_0;

public class Read : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBaseResource<BL.ResourceEntities.Profile> _profileResource;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Read(
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IBaseResource<BL.ResourceEntities.Profile> profileResource,
        IMapper mapper,
        IUserClaimsProfile userClaimsProfile
    )
    {
        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _profileResource = profileResource;
        _mapper = mapper;
        _userClaimsProfile = userClaimsProfile;
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

        var verificationResponse = _mapper.Map<ReadResponse>(InitDataCreator.Profiles.TestUser);

        var configuredResource = _profileResource
            .ReadSettings(settings => settings
                .Where(profile =>
                    profile.UserId == Guid.Parse(_userClaimsProfile.UserId))
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
}
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
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.Contracts.MyOrder.V1_0_0;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace BookStore.OrderService.IntegrationTests.Tests.Resources.MyOrders.V1_0_0;

public class Read : IDisposable
{
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IMapper _mapper;
    private readonly IBaseResourceCollection<Order> _orderResourceCollection;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Read(
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        IMapper mapper,
        IUserClaimsProfile userClaimsProfile,
        IBaseResourceCollection<Order> orderResourceCollection
    )
    {
        _dbContext = dbContext;
        _databaseInits = databaseInits;
        _mapper = mapper;
        _userClaimsProfile = userClaimsProfile;
        _orderResourceCollection = orderResourceCollection;
    }

    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task ReadByBook_DefaultInitData_MyOrdersIsRead()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        var verificationOrderOne = InitDataCreator.TestUserOrders.OrderOne;
        verificationOrderOne.Profile = InitDataCreator.Profiles.TestUser;

        var verificationOrderTwo = InitDataCreator.TestUserOrders.OrderTwo;
        verificationOrderTwo.Profile = InitDataCreator.Profiles.TestUser;

        var verificationResponse = _mapper.Map<IEnumerable<ReadResponse>>(
            new[] {verificationOrderOne, verificationOrderTwo});

        var configuredResourceCollection = _orderResourceCollection
            .ReadSettings(sets => sets
                .Where(order => order.Profile.UserId ==
                                Guid.Parse(_userClaimsProfile.UserId))
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking());

        IEnumerable<ReadResponse> targetResult;
        try
        {
            targetResult = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResourceCollection.ReadAsync();
        }

        ;
        //Assert
        targetResult.Should().BeEquivalentTo(verificationResponse,
            sets => sets
                .Excluding(verResponse => verResponse.CreationDateTime)
                .Excluding(verResponse => verResponse.Books));
    }
}
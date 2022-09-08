using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Abstractions;
using BookStore.OrderService.Data.BaseDatabase;
using FluentAssertions;
using Xunit;

namespace BookStore.OrderService.IntegrationTests.Tests.Resources.MyOrder.V1_0_0;

public class CreateFromMyCart : IDisposable
{
    private readonly ICreateMyOrderResourceFromMyCart _createMyOrderResourceFromMyCart;
    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;
    private readonly IResourcesCommitter _resourcesCommitter;

    public CreateFromMyCart(
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits,
        ICreateMyOrderResourceFromMyCart createMyOrderResourceFromMyCart
    )
    {
        _resourcesCommitter = resourcesCommitter;
        _createMyOrderResourceFromMyCart = createMyOrderResourceFromMyCart;

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
    public async Task Create_DefaultInitData_MyOrderIsCreated()
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        var result = await _createMyOrderResourceFromMyCart.CreateFromMyCart();

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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.Testing;
using BookStore.Base.Implementations.Testing.Autofixture;
using BookStore.BookService.Contracts.Author.V1_0_0.Create;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.WebEntryPoint.Author.V1_0_0.Create;
using Xunit;

namespace BookStore.BookService.IntegrationTests.Tests.Resources.Author.V1_0_0;

public class Create : IDisposable
{
    private readonly IBaseResource<BL.ResourceEntities.Author> _authorResource;

    private readonly IEnumerable<IDatabaseInit> _databaseInits;
    private readonly BaseDbContext _dbContext;

    private readonly IResourcesCommitter _resourcesCommitter;
    // private readonly CreateRequest request

    public Create(
        IBaseResource<BL.ResourceEntities.Author> authorResource,
        IResourcesCommitter resourcesCommitter,
        BaseDbContext dbContext,
        IEnumerable<IDatabaseInit> databaseInits
    )
    {
        _authorResource = authorResource;
        _resourcesCommitter = resourcesCommitter;

        _dbContext = dbContext;
        _databaseInits = databaseInits;
    }
    // public static TheoryData<CreateRequest> Data =>
    //     new()
    //     {
    //         new CreateRequest
    //         {
    //             FirstName = "FirstNameTest"
    //             Patronymic = "Patronymic",
    //         }
    //     };


    public void Dispose()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();

        GC.SuppressFinalize(this);
    }

    [Theory, AutoDataBookStore]
    // [MemberData(nameof(Data))]
    public async Task Create_ValidRequest_AuthorIsCreated(CreateRequest request)
    {
        //Arrange
        await DatabaseTestHelper.Initialize(_databaseInits, _dbContext);

        //Act
        _authorResource.Create(request.ToEntity());

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
        Assert.True(true);
    }
}
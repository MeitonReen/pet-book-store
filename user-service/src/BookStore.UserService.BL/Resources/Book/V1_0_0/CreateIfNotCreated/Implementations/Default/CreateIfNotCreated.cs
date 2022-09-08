using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.BookExistence.V1_0_0.Read;
using BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Abstractions;

namespace BookStore.UserService.BL.Resources.Book.V1_0_0.CreateIfNotCreated.Implementations.Default;

public class CreateIfNotCreatedBookResource : ICreateIfNotCreatedBookResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResourceExistence<ResourceEntities.Book> _bookExistence;
    private readonly IBaseResource<ResourceEntities.Book> _bookResource;
    private readonly IBaseOuterResourceExistence _outerExistence;
    private readonly IResourcesCommitter _resourcesCommitter;

    public CreateIfNotCreatedBookResource(
        IBaseOuterResourceExistence outerExistence,
        IBaseResourceExistence<ResourceEntities.Book> bookExistence,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<ResourceEntities.Book> bookResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor
    )
    {
        _appConfig = appConfigAccessor.Value;
        _outerExistence = outerExistence;
        _bookExistence = bookExistence;
        _resourcesCommitter = resourcesCommitter;
        _bookResource = bookResource;
    }

    public async Task<ResultModel> CreateIfNotCreated(Guid targetBookId,
        Action<ResourceEntities.Book> fillIfCreate)
    {
        if (await _bookExistence.ReadSettings(book => book.BookId = targetBookId).ReadAsync())
            return ResultModelBuilder
                .Success("Ok")
                .Build()
                .ToResultModel();

        var bookPresenceInOuterResourceCollection =
            await _outerExistence
                .ReadSettings(sets => sets
                    .ReadMessage<ReadBookExistenceRequest>(
                        new Contracts.Book.V1_0_0.ReadOut.ReadBookExistenceRequest {BookId = targetBookId}))
                .ReadAsync();

        if (!bookPresenceInOuterResourceCollection)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Target book is not exists")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        var targetBook = _bookResource.Create(book => book.BookId = targetBookId).ResourceEntity;
        fillIfCreate(targetBook);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return ResultModelBuilder
            .Created(targetBook)
            .Build()
            .ToResultModel();
    }
}
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Abstractions;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Helpers.Abstractions;

namespace BookStore.OrderService.BL.Resources.Book.V1_0_0.Delete.Implementations.Default;

public class DeleteBookResource : IDeleteBookResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResourceExistence<ResourceEntities.Book> _bookExistence;
    private readonly IBaseResource<ResourceEntities.Book> _bookResource;
    private readonly IBaseResourceCollection<Cart> _cartResourceCollection;

    private readonly ICartResourceCollectionReadSettings _cartResourceCollectionReadSettings;

    public DeleteBookResource(
        IBaseResource<ResourceEntities.Book> bookResource,
        IBaseResourceCollection<Cart> cartResourceCollection,
        ICartResourceCollectionReadSettings cartResourceCollectionReadSettings,
        IBaseResourceExistence<ResourceEntities.Book> bookExistence,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor
    )
    {
        _appConfig = appConfigAccessor.Value;
        _bookResource = bookResource;
        _cartResourceCollection = cartResourceCollection;
        _cartResourceCollectionReadSettings = cartResourceCollectionReadSettings;
        _bookExistence = bookExistence;
    }

    public async Task<ResultModel> Delete(Guid bookId)
    {
        if (!await _bookExistence
                .ReadSettings(book => !book.Deleted && book.BookId == bookId)
                .ReadAsync())
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Book not found")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        _bookResource.Update(book => book.BookId = bookId, book => book.Deleted = true);

        var cartsWithDeletedBook = await _cartResourceCollection
            .ReadSettings(sets =>
                _cartResourceCollectionReadSettings.CartsWithDeletedBook(sets, bookId))
            .ReadAsync();

        Array.ForEach(cartsWithDeletedBook.ToArray(), el => el.Books.Remove(el.Books.Single()));

        return ResultModelBuilder
            .Deleted(true)
            .Build()
            .ToResultModel();
    }
}
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Abstractions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;
using BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Implementation;

public class CreateBookInMyCartResource : ICreateBookInMyCartResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookInCart> _bookInCartResource;
    private readonly IBaseOuterResource<ResourceEntities.Book> _bookOuterResource;
    private readonly IBaseResource<ResourceEntities.Book> _bookResource;
    private readonly IBaseResource<Cart> _cartResource;

    private readonly IMyCartResourceReadSettings _myCartResourceReadSettings;

    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateBookInMyCartResource(
        IUserClaimsProfile userClaimsProfile,
        IBaseResource<Cart> cartResource,
        IBaseResource<ResourceEntities.Book> bookResource,
        IBaseResource<BookInCart> bookInCartResource,
        IResourcesCommitter resourcesCommitter,
        IMyCartResourceReadSettings myCartResourceReadSettings,
        IBaseOuterResource<ResourceEntities.Book> bookOuterResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor
    )
    {
        _appConfig = appConfigAccessor.Value;
        _userClaimsProfile = userClaimsProfile;
        _cartResource = cartResource;
        _bookResource = bookResource;
        _bookInCartResource = bookInCartResource;
        _resourcesCommitter = resourcesCommitter;
        _myCartResourceReadSettings = myCartResourceReadSettings;
        _bookOuterResource = bookOuterResource;
    }

    public async Task<ResultModel> CreateBook(CreateBookRequest request)
    {
        var targetBook = new ResourceEntities.Book {BookId = request.BookId};

        var targetBookPresenceCheckResult = await CreateBookIfNotCreated(targetBook.BookId,
            createdBook => targetBook = createdBook);

        if (!targetBookPresenceCheckResult.IsCreated
            && !targetBookPresenceCheckResult.IsSuccess) return targetBookPresenceCheckResult;

        var myCart = await ReadMyCartIncludingRequestedBookInCart(targetBook.BookId,
            Guid.Parse(_userClaimsProfile.UserId));

        if (myCart == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Cart not found")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        if (!myCart.Books.Any())
        {
            CreateBookToMyCart(targetBook, myCart);
        }
        else
        {
            UpdateBookInMyCart(myCart);
        }

        return ResultModelBuilder
            .Created(new CreateBookResponse {BookId = request.BookId})
            .Build()
            .ToResultModel();
    }

    private void UpdateBookInMyCart(Cart myCart)
    {
        _bookInCartResource.Update(myCart.Books.Single(), bookInCart => bookInCart.Count++);
    }

    private Cart CreateBookToMyCart(ResourceEntities.Book targetBook, Cart myCart)
    {
        _bookInCartResource
            .Create(bookInCart => bookInCart.Count = 1)
            .CreateReference(path => path.Book, targetBook)
            .CreateReference(path => path.Cart, myCart);

        return myCart;
    }

    private async Task<Cart?> ReadMyCartIncludingRequestedBookInCart(Guid targetBookId,
        Guid myProfileId)
    {
        var configuredResource = _cartResource
            .ReadSettings(sets => _myCartResourceReadSettings
                .MyCartIncludingRequestedBookInCart(sets, targetBookId, myProfileId)
            );

        Cart? myCart;
        try
        {
            myCart = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            myCart = await configuredResource.ReadAsync();
        }

        return myCart;
    }

    private async Task<ResultModel> CreateBookIfNotCreated(Guid targetBookId,
        Action<ResourceEntities.Book> fillIfCreate)
    {
        var targetBook = await _bookResource
            .ReadSettings(sets => sets
                .Where(book => book.BookId == targetBookId)
                .Select(book => new ResourceEntities.Book {BookId = book.BookId, Deleted = book.Deleted})
                .AsNoTracking())
            .ReadAsync();

        switch (targetBook)
        {
            case {Deleted: false}:
                return ResultModelBuilder
                    .Success("Ok")
                    .Build()
                    .ToResultModel();
            case {Deleted: true}:
                return ResultModelBuilder
                    .BadRequest()
                    .ApplyDefaultSettings("Target book is deleted")
                    .Environment(_appConfig.Environment)
                    .Build()
                    .ToResultModel();
        }

        var targetBookFromOuterResource = await _bookOuterResource.ReadSettings(sets => sets
                .ReadMessage<ReadBookRequest>(
                    new Contracts.Book.V1_0_0.ReadOut.ReadBookRequest {BookId = targetBookId})
                .ResultMessage<ReadBookResponse>()
                .ConverterToResourceEntity(resultMessage => resultMessage.ToEntity()))
            .ReadAsync();

        if (targetBookFromOuterResource == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Target book is not exists")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        targetBook = _bookResource.Create(targetBookFromOuterResource).ResourceEntity;
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

public static class RequestedBookDataByOrderServiceResponseExtensions
{
    public static ResourceEntities.Book ToEntity(
        this ReadBookResponse resultMessage)
        => new()
        {
            BookId = resultMessage.BookId,
            Name = resultMessage.Name,
            Price = resultMessage.Price,
            PublicationDate = resultMessage.PublicationDate,
            Deleted = false
        };
}
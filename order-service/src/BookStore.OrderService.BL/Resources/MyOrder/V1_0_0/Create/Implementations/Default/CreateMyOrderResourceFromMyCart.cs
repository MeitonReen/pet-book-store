using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;
using BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Abstractions;
using BookStore.OrderService.Contracts.MyOrder.V1_0_0;

namespace BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Implementations.Default;

public class CreateMyOrderResourceFromMyCart : ICreateMyOrderResourceFromMyCart
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BookInCart> _bookInCartResource;
    private readonly IBaseResource<BookInOrder> _bookInOrderResource;
    private readonly IBaseResource<Cart> _cartResource;
    private readonly IMyCartResourceReadSettings _myCartResourceReadSettings;
    private readonly IBaseResource<Order> _orderResource;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateMyOrderResourceFromMyCart(
        IUserClaimsProfile userClaimsProfile,
        IBaseResource<Order> orderResource,
        IResourcesCommitter resourcesCommitter,
        IBaseResource<Cart> cartResource,
        IMyCartResourceReadSettings myCartResourceReadSettings,
        IBaseResource<BookInOrder> bookInOrderResource,
        IBaseResource<BookInCart> bookInCartResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor
    )
    {
        _appConfig = appConfigAccessor.Value;
        _userClaimsProfile = userClaimsProfile;
        _orderResource = orderResource;
        _cartResource = cartResource;
        _myCartResourceReadSettings = myCartResourceReadSettings;
        _bookInOrderResource = bookInOrderResource;
        _bookInCartResource = bookInCartResource;
        _resourcesCommitter = resourcesCommitter;
    }

    public async Task<ResultModel> CreateFromMyCart()
    {
        var myProfile = new Profile {UserId = Guid.Parse(_userClaimsProfile.UserId)};
        var myCart = await ReadMyCartIncludingBooks(myProfile.UserId);

        if (myCart == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Cart not found")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        if (!myCart.Books.Any())
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("Cart is empty")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        var newOrder = _orderResource
            .Create(newOrder =>
            {
                newOrder.CreationDateTime = DateTime.Now;
                newOrder.Amount = 0;
            })
            .CreateReference(path => path.Profile, myProfile)
            .ResourceEntity;

        newOrder = myCart.Books
            .Aggregate(newOrder, (newOrderInner, bookInMyCart) =>
            {
                var bookToOrder = _bookInOrderResource
                    .Create(bookInMyCart.ToBookInOrder())
                    .CreateReference(path => path.Book, bookInMyCart.Book)
                    .ResourceEntity;

                _orderResource
                    .CreateReference(newOrderInner, path => path.Books, bookToOrder)
                    .Update(order => order.Amount += bookInMyCart.Book.Price * bookInMyCart.Count);

                return newOrderInner;
            });

        CreateCheckoutMyCart(myCart, myProfile);

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
            .Created(new CreateFromMyCartResponse {OrderId = newOrder.OrderId})
            .Build()
            .ToResultModel();
    }

    private void CreateCheckoutMyCart(Cart myOldCart, Profile myProfile)
    {
        //for each new my cart – single cartId from hash(myProfileId)
        _cartResource
            .Create(myNewCart =>
            {
                myNewCart.CheckoutDateTime = DateTime.Now;
                myNewCart.CreationDateTime = myOldCart.CreationDateTime;
            })
            .CreateReference(path => path.Profile, myProfile)
            .CreateReferences(path => path.Books, myOldCart.Books.Select(bookInCartOld =>
                _bookInCartResource
                    .Create(bookInCartOld.ToNewBookInCart())
                    .CreateReference(path => path.Book, bookInCartOld.Book)
                    .ResourceEntity));

        _cartResource.Delete(myOldCart);
    }

    private async Task<Cart?> ReadMyCartIncludingBooks(Guid myProfileId)
    {
        var configuredResource = _cartResource
            .ReadSettings(sets => _myCartResourceReadSettings
                .MyCartIncludingBooks(sets, myProfileId));

        Cart? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        return targetResult;
    }
}

public static class BookInCartExtensions
{
    public static BookInCart ToNewBookInCart(this BookInCart targetEntity)
        => new() {Count = targetEntity.Count};

    public static BookInOrder ToBookInOrder(this BookInCart targetEntity)
        => new() {Count = targetEntity.Count};
}
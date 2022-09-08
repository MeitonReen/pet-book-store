using System.Security.Cryptography;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Builders.Conflict.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.InterserviceContracts.UserService.V1_0_0.ProfileExistence.V1_0_0.Read;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Abstractions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;
using BookStore.OrderService.Contracts.MyCart.V1_0_0;

namespace BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Implementation;

public class CreateMyCartResource : ICreateMyCartResource
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<Cart> _cartResource;

    private readonly IMyCartResourceReadSettings _myCartResourceReadSettings;
    private readonly IBaseOuterResourceExistence _outerResourceExistence;

    private readonly IBaseResourceExistence<Profile> _profileExistence;
    private readonly IBaseResource<Profile> _profileResource;

    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public CreateMyCartResource(
        IUserClaimsProfile userClaimsProfile,
        IBaseResource<Cart> cartResource,
        IResourcesCommitter resourcesCommitter,
        IMyCartResourceReadSettings myCartResourceReadSettings,
        IBaseResourceExistence<Profile> profileExistence,
        IBaseOuterResourceExistence outerResourceExistence,
        IBaseResource<Profile> profileResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor
    )
    {
        _appConfig = appConfigAccessor.Value;
        _userClaimsProfile = userClaimsProfile;
        _cartResource = cartResource;
        _resourcesCommitter = resourcesCommitter;
        _myCartResourceReadSettings = myCartResourceReadSettings;
        _profileExistence = profileExistence;
        _outerResourceExistence = outerResourceExistence;
        _profileResource = profileResource;
    }

    public async Task<ResultModel> Create()
    {
        var myProfile = new Profile {UserId = Guid.Parse(_userClaimsProfile.UserId)};

        var targetProfilePresenceCheck = await CreateProfileIfNotCreated(myProfile.UserId,
            createdProfile => myProfile = createdProfile);

        if (!targetProfilePresenceCheck.IsCreated
            && !targetProfilePresenceCheck.IsSuccess) return targetProfilePresenceCheck;

        var myCart = await ReadMyCart(myProfile.UserId);

        if (myCart != default)
            return ResultModelBuilder
                .Conflict()
                .ApplyDefaultSettings("My cart already created")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        var newCart = _cartResource
            .Create(newCart =>
            {
                newCart.CartId = new Guid(ConcurrencyCreateProtect(myProfile.UserId));
                newCart.CreationDateTime = DateTime.Now;
            })
            .CreateReference(newCartRef => newCartRef.Profile, myProfile)
            .ResourceEntity;

        return ResultModelBuilder
            .Created(new CreateResponse {CartId = newCart.CartId})
            .Build()
            .ToResultModel();
    }

    private async Task<Cart?> ReadMyCart(Guid myProfileId)
    {
        var configuredResource = _cartResource
            .ReadSettings(sets => _myCartResourceReadSettings
                .MyCartShort(sets, myProfileId));

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

    private byte[] ConcurrencyCreateProtect(Guid myProfileId)
    {
        //for each new my cart – single cartId, from hash(myProfileId)
        using var sha256Hash = SHA256.Create();
        return sha256Hash
            .ComputeHash(myProfileId.ToByteArray())
            .TakeWhile((_, i) => i < 16)
            .ToArray();
    }

    private async Task<ResultModel> CreateProfileIfNotCreated(Guid myProfileId,
        Action<Profile> fillIfCreate)
    {
        if (await _profileExistence
                .ReadSettings(profile => profile.UserId = myProfileId)
                .ReadAsync())
            return ResultModelBuilder
                .Success("Ok")
                .Build()
                .ToResultModel();

        if (!await _outerResourceExistence
                .ReadSettings(sets => sets
                    .ReadMessage<ReadProfileExistenceRequest>(
                        new Contracts.ProfileExistence.V1_0_0.ReadOut.ReadProfileExistenceRequest
                            {UserId = myProfileId}))
                .ReadAsync())
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("User profile not found")
                .Environment(_appConfig.Environment)
                .Build()
                .ToResultModel();

        var targetProfile = _profileResource
            .Create(profile => profile.UserId = myProfileId)
            .ResourceEntity;
        fillIfCreate(targetProfile);

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
            .Created(targetProfile)
            .Build()
            .ToResultModel();
    }
}
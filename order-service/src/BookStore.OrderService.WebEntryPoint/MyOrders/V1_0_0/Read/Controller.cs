using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.NonAttachedExtensions;
using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.Contracts.MyOrder.V1_0_0;
using BookStore.OrderService.Contracts.MyOrders.V1_0_0.Read;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;
using HttpApiRouteBuilder = BookStore.OrderService.Contracts.MyOrders.V1_0_0.HttpApiRouteBuilder;

namespace BookStore.OrderService.WebEntryPoint.MyOrders.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IConfigurationProvider _mapperConfigurationProvider;
    private readonly IBaseResourceCollection<Order> _orderResourceCollection;
    private readonly IBaseResourceCollectionCount<Order> _orderResourceCollectionCount;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Controller(
        IUserClaimsProfile userClaimsProfile,
        IBaseResourceCollection<Order> orderResourceCollection,
        IConfigurationProvider mapperConfigurationProvider,
        IBaseResourceCollectionCount<Order> orderResourceCollectionCount,
        ILogger<Controller>? logger = default
    )
        : base(logger)
    {
        _userClaimsProfile = userClaimsProfile;
        _orderResourceCollection = orderResourceCollection;
        _mapperConfigurationProvider = mapperConfigurationProvider;
        _orderResourceCollectionCount = orderResourceCollectionCount;
    }

    /// <summary>
    /// Read my orders
    /// </summary>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{R}")]
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyOrders.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _orderResourceCollection
            .ReadSettings(sets => sets
                .Where(order => order.Profile.UserId ==
                                Guid.Parse(_userClaimsProfile.UserId))
                .Share(readSettings => _orderResourceCollectionCount
                    .ResourceCollectionSettings(_ => readSettings))
                .ReadPart(request)
                .OrderBy(order => order.OrderId)
                .ProjectTo<ReadResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        ICollection<ReadResponse> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }

        var targetResult = targetResource
            .ToDataPartResponse(request, await _orderResourceCollectionCount
                .ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}
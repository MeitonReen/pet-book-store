using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.OrderService.BL.Resources.MyOrder.V1_0_0.Create.Abstractions;
using BookStore.OrderService.Contracts.MyOrder.V1_0_0;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.OrderService.WebEntryPoint.MyOrder.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly ICreateMyOrderResourceFromMyCart _createMyOrderResourceFromMyCart;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IResourcesCommitter resourcesCommitter,
        ICreateMyOrderResourceFromMyCart createMyOrderResourceFromMyCart,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _resourcesCommitter = resourcesCommitter;
        _createMyOrderResourceFromMyCart = createMyOrderResourceFromMyCart;
    }

    /// <summary>
    /// Create order from my cart
    /// </summary>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateFromMyCartResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyOrder.Build)]
    [HttpPost]
    public async Task<IActionResult> CreateFromMyCart()
    {
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

        return result.ToActionResult();
    }
}
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Create.Abstractions;
using BookStore.OrderService.Contracts.MyCart.V1_0_0;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.OrderService.WebEntryPoint.MyCart.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly ICreateMyCartResource _createMyCartResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        ICreateMyCartResource createMyCartResource,
        IResourcesCommitter resourcesCommitter,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _createMyCartResource = createMyCartResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Create my cart
    /// </summary>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyCart.Build)]
    [HttpPost]
    public async Task<IActionResult> Create()
    {
        var result = await _createMyCartResource.Create();

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
using System.ComponentModel.DataAnnotations;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Book.Create.Abstractions;
using BookStore.OrderService.Contracts.MyCart.V1_0_0;
using BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.OrderService.WebEntryPoint.MyCart.V1_0_0.Book.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly ICreateBookInMyCartResource _createBookInMyCartResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        ICreateBookInMyCartResource createBookInMyCartResource,
        IResourcesCommitter resourcesCommitter,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _createBookInMyCartResource = createBookInMyCartResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Create a book in my cart
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateBookResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyCart.Book.Build)]
    [HttpPost]
    public async Task<IActionResult> CreateBook(
        [FromBody] [Required] CreateBookRequest request)
    {
        var result = await _createBookInMyCartResource.CreateBook(request);

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
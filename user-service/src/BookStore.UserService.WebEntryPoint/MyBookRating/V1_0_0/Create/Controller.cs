using System.ComponentModel.DataAnnotations;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Create.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookRating.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly ICreateMyBookRatingResource _createMyBookRatingResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IResourcesCommitter resourcesCommitter,
        ICreateMyBookRatingResource createMyBookRatingResource,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _createMyBookRatingResource = createMyBookRatingResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Create my rating for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookRating.Build)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] [Required] CreateRequest request)
    {
        var result = await _createMyBookRatingResource.Create(request);

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
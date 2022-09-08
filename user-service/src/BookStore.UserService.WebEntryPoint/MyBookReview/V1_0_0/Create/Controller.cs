using System.ComponentModel.DataAnnotations;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Create.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookReview.V1_0_0.Create;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly ICreateMyBookReviewResource _createMyBookReviewResource;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IResourcesCommitter resourcesCommitter,
        ICreateMyBookReviewResource createMyBookReviewResource,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _createMyBookReviewResource = createMyBookReviewResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Create my review for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="201">Successfully created</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{C}")]
    [ProducesResponseType(typeof(CreateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookReview.Build)]
    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] [Required] CreateRequest request)
    {
        var result = await _createMyBookReviewResource.Create(request);

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
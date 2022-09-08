using System.ComponentModel.DataAnnotations;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Update.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookReview.V1_0_0.Update;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUpdateMyBookReviewResource _updateMyBookReviewResource;

    public Controller(
        IResourcesCommitter resourcesCommitter,
        IUpdateMyBookReviewResource updateMyBookReviewResource,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _updateMyBookReviewResource = updateMyBookReviewResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Update my review for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully updated</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{U}")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookReview.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update(
        [FromBody] [Required] UpdateRequest request)
    {
        var result = await _updateMyBookReviewResource.Update(request);

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
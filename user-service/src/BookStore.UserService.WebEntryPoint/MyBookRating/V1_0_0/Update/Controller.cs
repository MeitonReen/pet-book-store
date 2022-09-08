using System.ComponentModel.DataAnnotations;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Update.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookRating.V1_0_0.Update;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUpdateMyBookRatingResource _updateMyBookRatingResource;

    public Controller(
        IResourcesCommitter resourcesCommitter,
        IUpdateMyBookRatingResource updateMyBookRatingResource,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _updateMyBookRatingResource = updateMyBookRatingResource;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Update my rating for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully updated</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{U}")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookRating.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update(
        [FromBody] [Required] UpdateRequest request)
    {
        var result = await _updateMyBookRatingResource.Update(request);

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
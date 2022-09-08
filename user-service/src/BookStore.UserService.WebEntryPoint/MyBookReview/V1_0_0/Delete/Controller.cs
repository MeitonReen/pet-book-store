using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0;
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookReview.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BL.ResourceEntities.BookReview> _bookReviewResource;
    private readonly IReadShortMyBookReviewResource _readShortMyBookReviewResource;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Controller(
        IBaseResource<BL.ResourceEntities.BookReview> bookReviewResource,
        IResourcesCommitter resourcesCommitter,
        IUserClaimsProfile userClaimsProfile,
        IReadShortMyBookReviewResource readShortMyBookReviewResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _appConfig = appConfigAccessor.Value;
        _readShortMyBookReviewResource = readShortMyBookReviewResource;
        _bookReviewResource = bookReviewResource;
        _resourcesCommitter = resourcesCommitter;
        _userClaimsProfile = userClaimsProfile;
    }

    /// <summary>
    /// Delete my review for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{D}")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookReview.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteRequest request)
    {
        var userProfileId = Guid.Parse(_userClaimsProfile.UserId);

        var targetBookReview = await _readShortMyBookReviewResource.ReadShort(
            request.BookId, userProfileId);

        if (targetBookReview == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("")
                .Environment(_appConfig.Environment)
                .Build()
                .ToActionResult();

        _bookReviewResource.Delete(targetBookReview);
        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e) when
            (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        return ResultModelBuilder
            .Deleted(new DeleteResponse
            {
                BookId = request.BookId,
                ReviewId = targetBookReview.ReviewId
            })
            .Build()
            .ToActionResult();
    }
}
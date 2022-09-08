using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookRating.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly AppConfig _appConfig;
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IReadShortMyBookRatingResource _readShortMyBookRatingResource;
    private readonly IResourcesCommitter _resourcesCommitter;
    private readonly IUserClaimsProfile _userClaimsProfile;


    public Controller(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        IResourcesCommitter resourcesCommitter,
        IUserClaimsProfile userClaimsProfile,
        IReadShortMyBookRatingResource readShortMyBookRatingResource,
        IOptionsSnapshotMixOptionsMonitor<AppConfig> appConfigAccessor,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _appConfig = appConfigAccessor.Value;
        _readShortMyBookRatingResource = readShortMyBookRatingResource;
        _bookRatingResource = bookRatingResource;
        _resourcesCommitter = resourcesCommitter;
        _userClaimsProfile = userClaimsProfile;
    }

    /// <summary>
    /// Delete my rating for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{D}")]
    [ProducesResponseType(typeof(DeleteResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookRating.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteRequest request)
    {
        var userProfileId = Guid.Parse(_userClaimsProfile.UserId);

        var targetBookRating = await _readShortMyBookRatingResource.ReadShort(
            request.BookId, userProfileId);

        if (targetBookRating == default)
            return ResultModelBuilder
                .BadRequest()
                .ApplyDefaultSettings("")
                .Environment(_appConfig.Environment)
                .Build()
                .ToActionResult();

        _bookRatingResource.Delete(targetBookRating);
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
                RatingId = targetBookRating.RatingId
            })
            .Build()
            .ToActionResult();
    }
}
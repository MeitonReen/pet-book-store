using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.NotFound.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0;
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Read;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.MyBookRating.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IMapper _mapper;
    private readonly IUserClaimsProfile _userClaimsProfile;

    public Controller(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        IMapper mapper,
        IUserClaimsProfile userClaimsProfile,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookRatingResource = bookRatingResource;
        _mapper = mapper;
        _userClaimsProfile = userClaimsProfile;
    }

    /// <summary>
    /// Read my rating for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultUserResources}{R}")]
    [ProducesResponseType(typeof(ReadResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.MyBookRating.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadRequest request)
    {
        var configuredResource = _bookRatingResource
            .ReadSettings(sets => sets
                .Where(bookRating =>
                    bookRating.Book != default
                    && bookRating.Book.BookId == request.BookId
                    && bookRating.Profile.UserId == Guid.Parse(_userClaimsProfile.UserId))
                .ProjectTo<ReadResponse>(_mapper.ConfigurationProvider)
                .AsNoTracking()
            );

        ReadResponse? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        return targetResult != default
            ? ResultModelBuilder
                .Read(targetResult)
                .Build()
                .ToActionResult()
            : ResultModelBuilder
                .NotFound()
                .ApplyDefaultSettings()
                .Build()
                .ToActionResult();
    }
}
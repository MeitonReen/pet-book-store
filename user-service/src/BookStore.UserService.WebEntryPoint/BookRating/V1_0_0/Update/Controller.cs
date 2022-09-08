using System.ComponentModel.DataAnnotations;
using AutoMapper;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.BookRating.V1_0_0;
using BookStore.UserService.Contracts.BookRating.V1_0_0.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.BookRating.V1_0_0.Update;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResource<BL.ResourceEntities.BookRating> _bookRatingResource;
    private readonly IMapper _mapper;
    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResource<BL.ResourceEntities.BookRating> bookRatingResource,
        IResourcesCommitter resourcesCommitter,
        IMapper mapper,
        ILogger<Microsoft.AspNetCore.Mvc.Controller>? logger = default
    ) : base(logger)
    {
        _bookRatingResource = bookRatingResource;
        _resourcesCommitter = resourcesCommitter;
        _mapper = mapper;
    }

    /// <summary>
    /// Update a book rating
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully updated</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{U}")]
    [ProducesResponseType(typeof(UpdateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookRating.Build)]
    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] [Required] UpdateRequest request)
    {
        var targetResource = _bookRatingResource.Update(request.ToEntity()).ResourceEntity;

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResourceToResult = _mapper.Map<UpdateResponse>(targetResource);

        return ResultModelBuilder
            .Updated(targetResourceToResult)
            .Build()
            .ToActionResult();
    }
}

public static class CreateForBookRequestExtensions
{
    public static BL.ResourceEntities.BookRating ToEntity(this UpdateRequest request)
        => new()
        {
            RatingId = request.RatingId,
            Rating = request.Rating,
            DateTimeSet = DateTime.Now
        };
}
using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Contracts.Implementations.DataPart.V1_0_0.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.Base.NonAttachedExtensions;
using BookStore.UserService.Contracts.BookReview.V1_0_0.Read;
using BookStore.UserService.Contracts.BookReviews.V1_0_0;
using BookStore.UserService.Contracts.BookReviews.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.UserService.WebEntryPoint.BookReviews.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookReview>
        _bookReviewResourceCollection;

    private readonly IBaseResourceCollectionCount<BL.ResourceEntities.BookReview>
        _bookReviewResourceCollectionCount;

    private readonly IConfigurationProvider _mapperConfigurationProvider;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.BookReview> bookReviewResourceCollection,
        IConfigurationProvider mapperConfigurationProvider,
        IBaseResourceCollectionCount<BL.ResourceEntities.BookReview>
            bookReviewResourceCollectionCount,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookReviewResourceCollection = bookReviewResourceCollection;
        _bookReviewResourceCollectionCount = bookReviewResourceCollectionCount;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Read reviews for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookReviews.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _bookReviewResourceCollection
            .ReadSettings(sets => sets
                .Where(bookReview => bookReview.Book != default
                                     && bookReview.Book.BookId == request.BookId)
                .Share(readSettings => _bookReviewResourceCollectionCount
                    .ResourceCollectionSettings(_ => readSettings))
                .OrderBy(bookReview => bookReview.ReviewId)
                .ReadPart(request)
                .ProjectTo<ReadResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        IEnumerable<ReadResponse> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync();
        }

        var targetResult = targetResource
            .ToDataPartResponse(request, await _bookReviewResourceCollectionCount
                .ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}
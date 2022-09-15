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
using BookStore.UserService.Contracts.BookRating.V1_0_0.Read;
using BookStore.UserService.Contracts.BookRatings.V1_0_0;
using BookStore.UserService.Contracts.BookRatings.V1_0_0.Read;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BookStore.UserService.WebEntryPoint.BookRatings.V1_0_0.Read;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookRating>
        _bookRatingResourceCollection;

    private readonly IBaseResourceCollectionCount<BL.ResourceEntities.BookRating>
        _bookRatingResourceCollectionCount;

    private readonly IConfigurationProvider _mapperConfigurationProvider;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.BookRating> bookRatingResourceCollection,
        IConfigurationProvider mapperConfigurationProvider,
        IBaseResourceCollectionCount<BL.ResourceEntities.BookRating>
            bookRatingResourceCollectionCount,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookRatingResourceCollection = bookRatingResourceCollection;
        _bookRatingResourceCollectionCount = bookRatingResourceCollectionCount;
        _mapperConfigurationProvider = mapperConfigurationProvider;
    }

    /// <summary>
    /// Read ratings for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully read</response>
    /// <response code="400">Validation error</response>
    /// <response code="500">Unknown error</response>
    [ProducesResponseType(typeof(BaseDataPartResponse<ReadResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookRatings.Build)]
    [HttpGet]
    public async Task<IActionResult> Read(
        [FromQuery] ReadPartRequest request)
    {
        var configuredResourceCollection = _bookRatingResourceCollection
            .ReadSettings(sets => sets
                .Where(bookRating => bookRating.Book != default
                                     && bookRating.Book.BookId == request.BookId)
                .Share(readSettings => _bookRatingResourceCollectionCount
                    .ResourceCollectionSettings(_ => readSettings))
                .OrderBy(bookRating => bookRating.RatingId)
                .ReadPart(request)
                .ProjectTo<ReadResponse>(_mapperConfigurationProvider)
                .AsNoTracking());

        ICollection<ReadResponse> targetResource;
        try
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResource = await configuredResourceCollection.ReadAsync(
                sets => sets.ToListAsync());
        }

        var targetResult = targetResource
            .ToDataPartResponse(request, await _bookRatingResourceCollectionCount
                .ReadAsync());

        return ResultModelBuilder
            .Read(targetResult)
            .Build()
            .ToActionResult();
    }
}
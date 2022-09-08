using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.BookRatings.V1_0_0;
using BookStore.UserService.Contracts.BookRatings.V1_0_0.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.BookRatings.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookRating>
        _bookRatingResourceCollection;

    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.BookRating> bookRatingResourceCollection,
        IResourcesCommitter resourcesCommitter,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookRatingResourceCollection = bookRatingResourceCollection;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Delete ratings for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{D}")]
    [ProducesResponseType(typeof(DeleteByBookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookRatings.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteRequest request)
    {
        var targetBookRatings = await _bookRatingResourceCollection
            .ReadSettings(sets => sets
                .Where(bookRating => bookRating.Book != default
                                     && bookRating.Book.BookId == request.BookId)
                .Select(bookRating => new BL.ResourceEntities.BookRating
                    {RatingId = bookRating.RatingId})
                .AsNoTracking())
            .ReadAsync();

        targetBookRatings = _bookRatingResourceCollection.Delete(targetBookRatings);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResult = targetBookRatings
            .Aggregate(new DeleteByBookResponse(), (bookRatingResponse, el) =>
            {
                bookRatingResponse.RatingIds.Add(el.RatingId);
                return bookRatingResponse;
            });

        return ResultModelBuilder
            .Deleted(targetResult)
            .Build()
            .ToActionResult();
    }
}
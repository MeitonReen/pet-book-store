using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.Base.Implementations.Controller;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Extensions;
using BookStore.UserService.Contracts.BookReviews.V1_0_0;
using BookStore.UserService.Contracts.BookReviews.V1_0_0.Delete;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static BookStore.AuthorizationService.Defaults.BookStoreDefaultResourcesWithDot;
using static BookStore.Base.Implementations.ResourcesAuthorization.Contracts.ResourcesPolicyConstants;

namespace BookStore.UserService.WebEntryPoint.BookReviews.V1_0_0.Delete;

[ApiController]
public class Controller : BookStoreControllerBase
{
    private readonly IBaseResourceCollection<BL.ResourceEntities.BookReview>
        _bookReviewResourceCollection;

    private readonly IResourcesCommitter _resourcesCommitter;

    public Controller(
        IBaseResourceCollection<BL.ResourceEntities.BookReview> bookReviewResourceCollection,
        IResourcesCommitter resourcesCommitter,
        ILogger<Controller>? logger = default
    ) : base(logger)
    {
        _bookReviewResourceCollection = bookReviewResourceCollection;
        _resourcesCommitter = resourcesCommitter;
    }

    /// <summary>
    /// Delete reviews for a book
    /// </summary>
    /// <param name="request"></param>
    /// <response code="200">Successfully deleted</response>
    /// <response code="400">Error</response>
    /// <response code="500">Unknown error</response>
    [Authorize($"{DefaultBookStoreResources}{D}")]
    [ProducesResponseType(typeof(DeleteByBookResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [Route(HttpApiRouteBuilder.BookReviews.Build)]
    [HttpDelete]
    public async Task<IActionResult> Delete(
        [FromQuery] DeleteRequest request)
    {
        var targetBookReviews = await _bookReviewResourceCollection
            .ReadSettings(sets => sets
                .Where(bookReview => bookReview.Book != default
                                     && bookReview.Book.BookId == request.BookId)
                .Select(bookReview => new BL.ResourceEntities.BookReview
                    {ReviewId = bookReview.ReviewId})
                .AsNoTracking())
            .ReadAsync();

        targetBookReviews = _bookReviewResourceCollection.Delete(targetBookReviews);

        try
        {
            await _resourcesCommitter.CommitAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatCommit(e))
        {
            await _resourcesCommitter.CommitAsync();
        }

        var targetResult = targetBookReviews
            .Aggregate(new DeleteByBookResponse(), (bookReviewResponse, el) =>
            {
                bookReviewResponse.ReviewIds.Add(el.ReviewId);
                return bookReviewResponse;
            });

        return ResultModelBuilder
            .Deleted(targetResult)
            .Build()
            .ToActionResult();
    }
}
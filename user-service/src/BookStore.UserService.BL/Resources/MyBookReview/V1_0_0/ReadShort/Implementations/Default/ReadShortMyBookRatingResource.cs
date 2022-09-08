using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Helpers.Abstractions;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Implementations.Default;

public class ReadShortMyBookReviewResource : IReadShortMyBookReviewResource
{
    private readonly IBaseResource<BookReview> _bookReviewResource;
    private readonly IMyBookReviewResourceReadSettings _myBookReviewResourceReadSettings;

    public ReadShortMyBookReviewResource(
        IBaseResource<BookReview> bookReviewResource,
        IMyBookReviewResourceReadSettings myBookReviewResourceReadSettings
    )
    {
        _bookReviewResource = bookReviewResource;
        _myBookReviewResourceReadSettings = myBookReviewResourceReadSettings;
    }

    public async Task<BookReview?> ReadShort(Guid targetBookId,
        Guid myProfileId)
    {
        var configuredResource = _bookReviewResource
            .ReadSettings(sets => _myBookReviewResourceReadSettings
                .ReadMyBookReviewShortByBook(sets, targetBookId, myProfileId));

        BookReview? targetResult;
        try
        {
            targetResult = await configuredResource.ReadAsync();
        }
        catch (Exception e)
            when (BookStoreExceptionHandlingDefaults.WhenExpressions.RepeatRead(e))
        {
            targetResult = await configuredResource.ReadAsync();
        }

        return targetResult;
    }
}
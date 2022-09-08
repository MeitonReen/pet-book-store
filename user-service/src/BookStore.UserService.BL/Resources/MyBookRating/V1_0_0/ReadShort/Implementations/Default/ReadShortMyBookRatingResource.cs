using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.OtherHelpers;
using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Helpers.Abstractions;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Implementations.Default;

public class ReadShortMyBookRatingResource : IReadShortMyBookRatingResource
{
    private readonly IBaseResource<BookRating> _bookRatingResource;
    private readonly IMyBookRatingResourceReadSettings _myBookRatingResourceReadSettings;

    public ReadShortMyBookRatingResource(
        IBaseResource<BookRating> bookRatingResource,
        IMyBookRatingResourceReadSettings myBookRatingResourceReadSettings
    )
    {
        _bookRatingResource = bookRatingResource;
        _myBookRatingResourceReadSettings = myBookRatingResourceReadSettings;
    }

    public async Task<BookRating?> ReadShort(Guid targetBookId,
        Guid myProfileId)
    {
        var configuredResource = _bookRatingResource
            .ReadSettings(sets => _myBookRatingResourceReadSettings
                .ReadMyBookRatingShortByBook(sets, targetBookId, myProfileId));

        BookRating? targetResult;
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
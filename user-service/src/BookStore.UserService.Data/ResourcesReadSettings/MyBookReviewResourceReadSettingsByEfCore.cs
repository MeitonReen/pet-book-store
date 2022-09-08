using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Helpers.Abstractions;

namespace BookStore.UserService.Data.ResourcesReadSettings;

public class MyBookReviewResourceReadSettingsByEfCore : IMyBookReviewResourceReadSettings
{
    public IQueryable<BookReview> ReadMyBookReviewShortByBook(IQueryable<BookReview> readSettings,
        Guid targetBookId, Guid targetUserProfileId)
        => readSettings
            .Where(bookReview =>
                bookReview.Book != default
                && bookReview.Book.BookId == targetBookId
                && bookReview.Profile.UserId == targetUserProfileId)
            .Select(bookReview => new BookReview
                {ReviewId = bookReview.ReviewId});
}
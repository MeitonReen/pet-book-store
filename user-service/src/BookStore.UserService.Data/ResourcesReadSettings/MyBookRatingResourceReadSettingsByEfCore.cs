using BookStore.UserService.BL.ResourceEntities;
using BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Helpers.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.UserService.Data.ResourcesReadSettings;

public class MyBookRatingResourceReadSettingsByEfCore : IMyBookRatingResourceReadSettings
{
    public IQueryable<BookRating> ReadMyBookRatingShortByBook(IQueryable<BookRating> readSettings,
        Guid targetBookId, Guid targetUserProfileId)
        => readSettings
            .Where(bookRating =>
                bookRating.Book != default
                && bookRating.Book.BookId == targetBookId
                && bookRating.Profile.UserId == targetUserProfileId)
            .Select(bookRating => new BookRating
                {RatingId = bookRating.RatingId})
            .AsNoTracking();
}
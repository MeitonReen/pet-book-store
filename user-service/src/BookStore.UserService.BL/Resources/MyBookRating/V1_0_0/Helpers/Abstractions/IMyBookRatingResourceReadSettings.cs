using BookStore.UserService.BL.ResourceEntities;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.Helpers.Abstractions;

public interface IMyBookRatingResourceReadSettings
{
    IQueryable<BookRating> ReadMyBookRatingShortByBook(IQueryable<BookRating> readSettings,
        Guid targetBookId, Guid targetUserProfileId);
}
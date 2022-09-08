using BookStore.UserService.BL.ResourceEntities;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.Helpers.Abstractions;

public interface IMyBookReviewResourceReadSettings
{
    IQueryable<BookReview> ReadMyBookReviewShortByBook(IQueryable<BookReview> readSettings,
        Guid targetBookId, Guid targetUserProfileId);
}
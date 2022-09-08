using BookStore.UserService.BL.ResourceEntities;

namespace BookStore.UserService.BL.Resources.MyBookReview.V1_0_0.ReadShort.Abstractions;

public interface IReadShortMyBookReviewResource
{
    Task<BookReview?> ReadShort(Guid targetBookId, Guid myProfileId);
}
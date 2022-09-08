using BookStore.UserService.BL.ResourceEntities;

namespace BookStore.UserService.BL.Resources.MyBookRating.V1_0_0.ReadShort.Abstractions;

public interface IReadShortMyBookRatingResource
{
    Task<BookRating?> ReadShort(Guid targetBookId, Guid myProfileId);
}
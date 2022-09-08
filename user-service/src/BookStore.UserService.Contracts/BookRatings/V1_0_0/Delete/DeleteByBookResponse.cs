namespace BookStore.UserService.Contracts.BookRatings.V1_0_0.Delete
{
    public class DeleteByBookResponse
    {
        public List<Guid> RatingIds { get; set; } = new();
    }
}
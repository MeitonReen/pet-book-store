namespace BookStore.UserService.Contracts.BookReviews.V1_0_0.Delete
{
    public class DeleteByBookResponse
    {
        public List<Guid> ReviewIds { get; set; } = new();
    }
}
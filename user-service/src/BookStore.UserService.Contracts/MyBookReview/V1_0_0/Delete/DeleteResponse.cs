namespace BookStore.UserService.Contracts.MyBookReview.V1_0_0.Delete
{
    public class DeleteResponse
    {
        public Guid BookId { get; set; }
        public Guid ReviewId { get; set; }
    }
}
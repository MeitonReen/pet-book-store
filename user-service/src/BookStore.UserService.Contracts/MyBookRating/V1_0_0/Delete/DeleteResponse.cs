namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Delete
{
    public class DeleteResponse
    {
        public Guid BookId { get; set; }
        public Guid RatingId { get; set; }
    }
}
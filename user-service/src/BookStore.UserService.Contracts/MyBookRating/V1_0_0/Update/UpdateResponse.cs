namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update
{
    public class UpdateResponse
    {
        public Guid RatingId { get; set; }
        public int Rating { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Guid BookId { get; set; }
    }
}
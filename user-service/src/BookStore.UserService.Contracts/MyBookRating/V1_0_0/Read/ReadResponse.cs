namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Read
{
    public class ReadResponse
    {
        public Guid RatingId { get; set; }
        public int Rating { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Guid BookId { get; set; }
    }
}
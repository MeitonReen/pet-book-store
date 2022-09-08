namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create
{
    public class CreateResponse
    {
        public Guid RatingId { get; set; }
        public int Rating { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Guid BookId { get; set; }
    }
}
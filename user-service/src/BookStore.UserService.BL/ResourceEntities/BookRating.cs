namespace BookStore.UserService.BL.ResourceEntities
{
    public class BookRating
    {
        public Guid RatingId { get; set; }
        public int Rating { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Book? Book { get; set; } = default;
        public Profile Profile { get; set; } = default!;
    }
}
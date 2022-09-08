namespace BookStore.UserService.BL.ResourceEntities
{
    public class BookReview
    {
        public Guid ReviewId { get; set; }
        public string Review { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Book? Book { get; set; } = default!;
        public Profile Profile { get; set; } = default!;
    }
}
namespace BookStore.UserService.BL.ResourceEntities
{
    public class Profile
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Patronymic { get; set; }

        public ICollection<BookReview> BookReviews { get; set; } = new List<BookReview>();
        public ICollection<BookRating> BookRatings { get; set; } = new List<BookRating>();
        public static Profile Empty => new();
    }
}
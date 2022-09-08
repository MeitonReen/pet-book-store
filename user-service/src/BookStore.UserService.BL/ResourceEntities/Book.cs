namespace BookStore.UserService.BL.ResourceEntities;

public class Book
{
    public Guid BookId { get; set; }
    public ICollection<BookReview> BookReviews { get; set; } = new List<BookReview>();
    public ICollection<BookRating> BookRatings { get; set; } = new List<BookRating>();
}
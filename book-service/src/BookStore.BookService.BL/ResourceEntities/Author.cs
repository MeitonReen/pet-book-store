namespace BookStore.BookService.BL.ResourceEntities;

public class Author
{
    public Guid AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }

    public DateOnly BirthDate { get; set; }
    // public string? ImageUrl { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
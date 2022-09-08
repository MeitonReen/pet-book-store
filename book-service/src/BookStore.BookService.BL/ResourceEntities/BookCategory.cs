namespace BookStore.BookService.BL.ResourceEntities;

public class BookCategory
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Book> Books { get; set; } = new List<Book>();
}
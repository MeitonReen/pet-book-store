namespace BookStore.BookService.Contracts.Book.V1_0_0.Create;

public class CreateResponse
{
    public Guid BookId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public DateOnly PublicationDate { get; set; }
    // public string? ImageUrl { get; set; }
}
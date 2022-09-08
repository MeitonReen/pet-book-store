namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;

public class CreateBookReferenceResponse
{
    public Guid CategoryId { get; set; }
    public Guid BookId { get; set; }
}
namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;

public class CreateResponse
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
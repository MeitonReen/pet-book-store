namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Update;

public class UpdateResponse
{
    public Guid CategoryId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
}
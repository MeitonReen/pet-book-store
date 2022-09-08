namespace BookStore.BookService.Contracts.Book.V1_0_0.DeleteOut;

public class DeleteBookRequest : Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut.DeleteBookRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public DateOnly PublicationDate { get; set; }
    public Guid BookId { get; set; }
}
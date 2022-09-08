namespace BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update;

public interface UpdateBookCommand
{
    public Guid BookId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateOnly PublicationDate { get; set; }
    public decimal Price { get; set; }
}
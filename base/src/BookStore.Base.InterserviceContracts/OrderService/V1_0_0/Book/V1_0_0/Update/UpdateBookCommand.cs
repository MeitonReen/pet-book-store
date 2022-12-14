namespace BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Update;

public interface UpdateBookCommand
{
    public Guid BookId { get; set; }
    public string Name { get; set; }
    public DateOnly PublicationDate { get; set; }
    public decimal Price { get; set; }
}
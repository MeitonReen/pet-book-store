namespace BookStore.BookService.Contracts.Book.V1_0_0;

public class ReadBookResponse : Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut.ReadBookResponse
{
    public Guid BookId { get; set; }

    public string Name { get; set; }

    public DateOnly PublicationDate { get; set; }

    public decimal Price { get; set; }
}
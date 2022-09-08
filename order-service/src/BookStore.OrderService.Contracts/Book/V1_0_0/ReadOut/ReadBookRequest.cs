namespace BookStore.OrderService.Contracts.Book.V1_0_0.ReadOut;

public class ReadBookRequest : Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut.ReadBookRequest
{
    public Guid BookId { get; set; }
}
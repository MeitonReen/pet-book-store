namespace BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut;

public interface ReadBookRequest
{
    public Guid BookId { get; set; }
}
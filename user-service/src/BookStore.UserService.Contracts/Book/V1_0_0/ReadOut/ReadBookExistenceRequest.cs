namespace BookStore.UserService.Contracts.Book.V1_0_0.ReadOut;

public class ReadBookExistenceRequest : Base.InterserviceContracts.BookService.V1_0_0.BookExistence.V1_0_0.Read.
    ReadBookExistenceRequest
{
    public Guid BookId { get; set; }
}
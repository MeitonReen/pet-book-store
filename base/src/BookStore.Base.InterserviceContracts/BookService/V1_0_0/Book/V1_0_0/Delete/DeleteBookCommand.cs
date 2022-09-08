namespace BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;

public interface DeleteBookCommand
{
    public Guid BookId { get; set; }
}
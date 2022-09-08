using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.Delete;

namespace BookStore.OrderService.Contracts.Book.V1_0_0.Delete;

public class DeleteBookCompensateCommand : DeleteBookCommand
{
    public Guid BookId { get; set; }
}
using System.ComponentModel.DataAnnotations;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut;

namespace BookStore.BookService.Contracts.Book.V1_0_0.Delete;

public class DeleteRequest : DeleteBookRequest, DeleteBookCommand
{
    [Required] public Guid BookId { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Book.V1_0_0.Read;

public class ReadRequest
{
    [Required] public Guid BookId { get; set; }
}
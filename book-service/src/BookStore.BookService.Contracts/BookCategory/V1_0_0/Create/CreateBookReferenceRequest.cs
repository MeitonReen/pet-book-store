using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;

public class CreateBookReferenceRequest
{
    [Required] public Guid CategoryId { get; set; }
    [Required] public Guid BookId { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Read;

public class ReadRequest
{
    [Required] public Guid CategoryId { get; set; }
}
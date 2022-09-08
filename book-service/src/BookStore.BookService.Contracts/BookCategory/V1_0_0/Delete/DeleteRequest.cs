using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Delete;

public class DeleteRequest
{
    [Required] public Guid CategoryId { get; set; }
}
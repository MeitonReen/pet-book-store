using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Author.V1_0_0.Delete;

public class DeleteRequest
{
    [Required] public Guid AuthorId { get; set; }
}
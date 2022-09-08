using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Author.V1_0_0.Read;

public class ReadRequest
{
    [Required] public Guid AuthorId { get; set; }
}
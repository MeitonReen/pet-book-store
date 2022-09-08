using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Update;

public class UpdateRequest
{
    [Required] public Guid CategoryId { get; set; }
    [Required] public string Name { get; set; }
    public string Description { get; set; }
}
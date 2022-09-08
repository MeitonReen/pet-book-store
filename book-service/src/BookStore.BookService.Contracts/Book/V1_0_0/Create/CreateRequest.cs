using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Book.V1_0_0.Create;

public class CreateRequest
{
    [Required] public string Name { get; set; }
    public string Description { get; set; }
    [Required] public decimal Price { get; set; }

    [Required] public DateOnly PublicationDate { get; set; }
    // public IFormFile Image { get; set; }

    public Guid[] AuthorIds { get; set; } = Array.Empty<Guid>();
}
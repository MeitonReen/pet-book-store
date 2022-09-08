using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0.Create;

public class CreateRequest
{
    [Required] public string Name { get; set; }
    public string Description { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Author.V1_0_0.Create;

public class CreateRequest
{
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string Patronymic { get; set; }

    [Required] public DateOnly BirthDate { get; set; }
    // public IFormFile Image { get; set; }
}
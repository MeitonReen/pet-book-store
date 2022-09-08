using System.ComponentModel.DataAnnotations;

namespace BookStore.BookService.Contracts.Author.V1_0_0.Update;

public class UpdateRequest
{
    [Required] public Guid AuthorId { get; set; }
    [Required] public string FirstName { get; set; }
    [Required] public string LastName { get; set; }
    public string Patronymic { get; set; }
    [Required] public DateOnly BirthDate { get; set; }
}
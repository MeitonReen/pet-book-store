namespace BookStore.BookService.Contracts.Author.V1_0_0.Create;

public class CreateResponse
{
    public Guid AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }

    public DateOnly BirthDate { get; set; }
    // public string? ImageUrl { get; set; }
}
namespace BookStore.BookService.Contracts.Author.V1_0_0.Update;

public class UpdateResponse
{
    public Guid AuthorId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? Patronymic { get; set; }
    public DateOnly BirthDate { get; set; }
}
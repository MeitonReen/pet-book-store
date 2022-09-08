using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;
using BookStore.Base.Implementations;

namespace BookStore.BookService.Contracts.FilterableBooks.V1_0_0.Read;

public class ReadPartRequest : BaseDataPartRequest
{
    public string? AuthorLastName { get; set; }
    public string? CategoryName { get; set; }
    public SortOrder OrderPrice { get; set; }
}
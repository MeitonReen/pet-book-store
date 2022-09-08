using System.ComponentModel.DataAnnotations;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;

namespace BookStore.BookService.Contracts.Book.V1_0_0.Update;

public class UpdateRequest : UpdateBookRequest
{
    [Required] public Guid BookId { get; set; }
    [Required] public string Name { get; set; }
    public string Description { get; set; }
    [Required] public decimal Price { get; set; }
    [Required] public DateOnly PublicationDate { get; set; }
}
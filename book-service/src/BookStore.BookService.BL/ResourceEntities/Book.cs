using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Delete;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.DeleteOut;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.Update;
using BookStore.Base.InterserviceContracts.BookService.V1_0_0.Book.V1_0_0.UpdateOut;
using BookStore.Base.InterserviceContracts.OrderService.V1_0_0.Book.V1_0_0.ReadOut;

namespace BookStore.BookService.BL.ResourceEntities;

public class Book : UpdateBookRequest, ReadBookRequest, UpdateBookCommand, DeleteBookRequest, DeleteBookCommand
{
    // public string? ImageUrl { get; set; }

    public ICollection<BookCategory> Categories { get; set; }
        = new List<BookCategory>();

    public ICollection<Author> Authors { get; set; } = new List<Author>();
    public Guid BookId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly PublicationDate { get; set; }

    public decimal Price { get; set; }
}
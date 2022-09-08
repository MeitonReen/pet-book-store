using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.Book.V1_0_0.Helpers.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.Data.ResourcesReadSettings;

public class CartResourceCollectionReadSettings : ICartResourceCollectionReadSettings
{
    public IQueryable<Cart> CartsWithDeletedBook(IQueryable<Cart> readSettings, Guid bookId)
        => readSettings
            .Where(cart => cart.Books
                .Any(bookInCart => bookInCart.Book.BookId == bookId))
            .Select(cart => new Cart
            {
                CartId = cart.CartId,
                Books = cart.Books
                    .Select(bookInCart => new BookInCart
                    {
                        BookInCartId = bookInCart.BookInCartId,
                        Book = new Book {BookId = bookInCart.Book.BookId}
                    })
                    .Where(bookInCart => bookInCart.Book.BookId == bookId)
                    .ToList()
            })
            .AsNoTracking();
}
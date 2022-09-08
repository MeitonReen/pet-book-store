using BookStore.OrderService.BL.ResourceEntities;
using BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace BookStore.OrderService.Data.ResourcesReadSettings;

public class MyCartResourceReadSettingsByEfCore : IMyCartResourceReadSettings
{
    public IQueryable<Cart> MyCartShort(IQueryable<Cart> readSettings,
        Guid myProfileId)
        => readSettings
            .Where(cart => cart.Profile.UserId == myProfileId
                           && cart.CheckoutDateTime == default)
            .Select(cart => new Cart {CartId = cart.CartId})
            .AsNoTracking();

    public IQueryable<Cart> MyCartIncludingRequestedBookInCart(
        IQueryable<Cart> readSettings,
        Guid requestedBookId,
        Guid myProfileId)
        => readSettings
            .Where(cart => cart.Profile.UserId == myProfileId
                           && cart.CheckoutDateTime == default)
            .Select(cart => new Cart
            {
                CartId = cart.CartId, Books = cart.Books
                    .Where(bookInCart => !bookInCart.Book.Deleted
                                         && bookInCart.Book.BookId == requestedBookId)
                    .Select(bookInCart => new BookInCart
                    {
                        BookInCartId = bookInCart.BookInCartId,
                        Count = bookInCart.Count
                    }).ToList()
            })
            .AsNoTracking();

    public IQueryable<Cart> MyCartIncludingBooks(IQueryable<Cart> readSettings,
        Guid myProfileId)
        => readSettings
            .Where(cart => cart.Profile.UserId == myProfileId
                           && cart.CheckoutDateTime == default)
            .Select(cart => new Cart
            {
                CartId = cart.CartId,
                Books = cart.Books
                    .Where(bookInCart => !bookInCart.Book.Deleted)
                    .Select(bookInCart => new BookInCart
                    {
                        BookInCartId = bookInCart.BookInCartId,
                        Count = bookInCart.Count,
                        Book = new Book
                        {
                            BookId = bookInCart.Book.BookId,
                            Price = bookInCart.Book.Price
                        }
                    }).ToList(),
                CreationDateTime = cart.CreationDateTime
            })
            .AsNoTracking();
}
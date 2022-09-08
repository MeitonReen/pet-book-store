using BookStore.OrderService.BL.ResourceEntities;

namespace BookStore.OrderService.BL.Resources.Book.V1_0_0.Helpers.Abstractions;

public interface ICartResourceCollectionReadSettings
{
    IQueryable<Cart> CartsWithDeletedBook(IQueryable<Cart> readSettings, Guid bookId);
}
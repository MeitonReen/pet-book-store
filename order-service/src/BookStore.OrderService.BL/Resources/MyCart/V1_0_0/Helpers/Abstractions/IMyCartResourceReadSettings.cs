using BookStore.OrderService.BL.ResourceEntities;

namespace BookStore.OrderService.BL.Resources.MyCart.V1_0_0.Helpers.Abstractions;

public interface IMyCartResourceReadSettings
{
    IQueryable<Cart> MyCartShort(IQueryable<Cart> readSettings, Guid myProfileId);

    IQueryable<Cart> MyCartIncludingRequestedBookInCart(IQueryable<Cart> readSettings,
        Guid requestedBookId,
        Guid myProfileId);

    IQueryable<Cart> MyCartIncludingBooks(IQueryable<Cart> readSettings,
        Guid myProfileId);
}
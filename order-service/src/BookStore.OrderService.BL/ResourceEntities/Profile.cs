namespace BookStore.OrderService.BL.ResourceEntities;

public class Profile
{
    public Guid UserId { get; set; }

    public List<Cart> Carts { get; set; }
    public List<Order> Orders { get; set; }
}
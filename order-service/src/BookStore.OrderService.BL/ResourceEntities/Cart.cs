namespace BookStore.OrderService.BL.ResourceEntities
{
    public class Cart
    {
        public Guid CartId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public DateTime? CheckoutDateTime { get; set; }

        public Profile Profile { get; set; }
        public List<BookInCart> Books { get; set; } = new();
    }
}
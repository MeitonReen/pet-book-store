namespace BookStore.OrderService.BL.ResourceEntities
{
    public class BookInCart
    {
        public Guid BookInCartId { get; set; } = Guid.Empty;
        public int Count { get; set; } = 1;

        public Book Book { get; set; }
        public Cart Cart { get; set; }
    }
}
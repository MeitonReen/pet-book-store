namespace BookStore.OrderService.BL.ResourceEntities
{
    public class BookInOrder
    {
        public Guid BookInOrderId { get; set; } = Guid.Empty;
        public int Count { get; set; }

        public Book Book { get; set; }
        public Order Order { get; set; }
    }
}
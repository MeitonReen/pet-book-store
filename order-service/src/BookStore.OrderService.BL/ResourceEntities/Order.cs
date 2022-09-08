namespace BookStore.OrderService.BL.ResourceEntities
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public decimal Amount { get; set; }

        public Profile Profile { get; set; }
        public ICollection<BookInOrder> Books { get; set; } = new List<BookInOrder>();
    }
}
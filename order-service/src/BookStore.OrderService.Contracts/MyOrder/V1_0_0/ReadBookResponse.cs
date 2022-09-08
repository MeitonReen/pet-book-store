namespace BookStore.OrderService.Contracts.MyOrder.V1_0_0
{
    public class ReadBookResponse
    {
        public Guid BookId { get; set; }
        public string Name { get; set; }
        public DateOnly PublicationDate { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
    }
}
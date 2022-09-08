namespace BookStore.OrderService.Contracts.MyOrder.V1_0_0
{
    public class ReadResponse
    {
        public Guid OrderId { get; set; }
        public DateTime CreationDateTime { get; set; }
        public decimal Amount { get; set; }

        public IEnumerable<ReadBookResponse> Books { get; set; }
    }
}
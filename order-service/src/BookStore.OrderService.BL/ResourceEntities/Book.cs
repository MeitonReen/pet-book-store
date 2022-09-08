namespace BookStore.OrderService.BL.ResourceEntities
{
    public class Book
    {
        public Guid BookId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateOnly PublicationDate { get; set; }
        public decimal Price { get; set; }
        public bool Deleted { get; set; } = false;

        public ICollection<BookInCart> Carts { get; set; } = new List<BookInCart>();
        public ICollection<BookInOrder> Orders { get; set; } = new List<BookInOrder>();
    }
}
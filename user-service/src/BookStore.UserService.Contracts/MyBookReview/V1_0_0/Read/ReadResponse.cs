namespace BookStore.UserService.Contracts.MyBookReview.V1_0_0.Read
{
    public class ReadResponse
    {
        public Guid ReviewId { get; set; }
        public string Review { get; set; }
        public DateTime DateTimeSet { get; set; }

        public Guid BookId { get; set; }
    }
}
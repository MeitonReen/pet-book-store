namespace BookStore.Base.InterserviceContracts.BookService.V1_0_0.BookExistence.V1_0_0.Read
{
    public interface ReadBookExistenceRequest
    {
        public Guid BookId { get; set; }
    }
}
namespace BookStore.UserService.Contracts.Profile.V1_0_0.Create
{
    public class CreateResponse
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Patronymic { get; set; }
    }
}
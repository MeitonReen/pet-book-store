using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.Profile.V1_0_0.Create
{
    public class CreateRequest
    {
        [Required] public Guid UserId { get; set; }
        [Required] public string UserName { get; set; }

        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Patronymic { get; set; }
    }
}
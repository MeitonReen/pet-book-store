using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyProfile.V1_0_0.Update
{
    public class UpdateRequest
    {
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        public string Patronymic { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.Profile.V1_0_0.Read
{
    public class ReadRequest
    {
        [Required] public Guid UserId { get; set; }
    }
}
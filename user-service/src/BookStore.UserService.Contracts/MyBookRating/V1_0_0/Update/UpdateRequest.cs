using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update
{
    public class UpdateRequest
    {
        [Required] public Guid BookId { get; set; }
        [Required] public int Rating { get; set; }
    }
}
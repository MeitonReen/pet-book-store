using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create
{
    public class CreateRequest
    {
        [Required] public int Rating { get; set; }
        [Required] public Guid BookId { get; set; }
    }
}
using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookRating.V1_0_0.Update
{
    public class UpdateRequest
    {
        [Required] public Guid RatingId { get; set; }
        [Required] public int Rating { get; set; }
    }
}
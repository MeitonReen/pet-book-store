using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookRating.V1_0_0.Read
{
    public class ReadRequest
    {
        [Required] public Guid RatingId { get; set; }
    }
}
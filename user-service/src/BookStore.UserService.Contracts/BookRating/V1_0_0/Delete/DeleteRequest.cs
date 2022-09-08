using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookRating.V1_0_0.Delete
{
    public class DeleteRequest
    {
        [Required] public Guid RatingId { get; set; }
    }
}
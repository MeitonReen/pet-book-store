using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookReview.V1_0_0.Update
{
    public class UpdateRequest
    {
        [Required] public Guid ReviewId { get; set; }
        [Required] public string Review { get; set; }
    }
}
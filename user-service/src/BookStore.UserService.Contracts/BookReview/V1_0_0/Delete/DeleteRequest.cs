using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookReview.V1_0_0.Delete
{
    public class DeleteRequest
    {
        [Required] public Guid ReviewId { get; set; }
    }
}
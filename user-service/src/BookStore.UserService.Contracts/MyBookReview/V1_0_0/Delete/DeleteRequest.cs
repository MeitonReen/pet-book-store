using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyBookReview.V1_0_0.Delete
{
    public class DeleteRequest
    {
        [Required] public Guid BookId { get; set; }
    }
}
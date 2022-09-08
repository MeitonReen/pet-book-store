using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.BookReview.V1_0_0.Read
{
    public class ReadRequest
    {
        [Required] public Guid ReviewId { get; set; }
    }
}
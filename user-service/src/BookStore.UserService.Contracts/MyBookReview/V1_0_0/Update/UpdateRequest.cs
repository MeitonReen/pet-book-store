using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update
{
    public class UpdateRequest
    {
        [Required] public Guid BookId { get; set; }
        [Required] public string Review { get; set; }
    }
}
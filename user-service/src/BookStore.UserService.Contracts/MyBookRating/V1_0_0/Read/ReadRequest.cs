using System.ComponentModel.DataAnnotations;

namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0.Read
{
    public class ReadRequest
    {
        [Required] public Guid BookId { get; set; }
    }
}
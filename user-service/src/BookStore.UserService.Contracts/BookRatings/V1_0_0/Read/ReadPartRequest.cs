using System.ComponentModel.DataAnnotations;
using BookStore.Base.Contracts.Abstractions.DataPart.V1_0_0;

namespace BookStore.UserService.Contracts.BookRatings.V1_0_0.Read
{
    public class ReadPartRequest : BaseDataPartRequest
    {
        [Required] public Guid BookId { get; set; }
    }
}
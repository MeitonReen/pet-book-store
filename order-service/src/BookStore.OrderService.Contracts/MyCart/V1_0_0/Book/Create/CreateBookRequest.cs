using System.ComponentModel.DataAnnotations;

namespace BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create
{
    public class CreateBookRequest
    {
        [Required] public Guid BookId { get; set; }
    }
}
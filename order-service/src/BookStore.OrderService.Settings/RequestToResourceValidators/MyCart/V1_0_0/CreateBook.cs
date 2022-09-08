using BookStore.OrderService.Contracts.MyCart.V1_0_0.Book.Create;
using FluentValidation;

namespace BookStore.OrderService.Settings.RequestToResourceValidators.MyCart.V1_0_0
{
    public class CreateBook : AbstractValidator<CreateBookRequest>
    {
        public CreateBook()
        {
            RuleFor(input => input.BookId).NotEmpty();
        }
    }
}
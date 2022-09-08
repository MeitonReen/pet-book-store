using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Read;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyBookRating.V1_0_0
{
    public class ReadByBook : AbstractValidator<ReadRequest>
    {
        public ReadByBook()
        {
            RuleFor(input => input.BookId).NotEmpty();
        }
    }
}
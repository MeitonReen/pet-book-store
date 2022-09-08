using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Create;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyBookRating.V1_0_0
{
    public class CreateForBook : AbstractValidator<CreateRequest>
    {
        public CreateForBook()
        {
            RuleFor(input => input.BookId).NotEmpty();

            RuleFor(input => input.Rating).InclusiveBetween(1, 10);
        }
    }
}
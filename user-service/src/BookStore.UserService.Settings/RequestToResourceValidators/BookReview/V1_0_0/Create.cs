using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Create;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookReview.V1_0_0
{
    public class Create : AbstractValidator<CreateRequest>
    {
        public Create()
        {
            RuleFor(input => input.BookId).NotEmpty();

            RuleFor(input => input.Review)
                .NotEmpty()
                .Length(1000);
        }
    }
}
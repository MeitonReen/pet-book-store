using BookStore.UserService.Contracts.BookReview.V1_0_0.Update;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookReview.V1_0_0
{
    public class Update : AbstractValidator<UpdateRequest>
    {
        public Update()
        {
            RuleFor(input => input.ReviewId).NotEmpty();

            RuleFor(input => input.Review)
                .NotEmpty()
                .Length(1000);
        }
    }
}
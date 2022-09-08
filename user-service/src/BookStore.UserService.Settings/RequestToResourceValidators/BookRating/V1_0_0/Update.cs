using BookStore.UserService.Contracts.BookRating.V1_0_0.Update;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookRating.V1_0_0
{
    public class Update : AbstractValidator<UpdateRequest>
    {
        public Update()
        {
            RuleFor(input => input.RatingId).NotEmpty();
            RuleFor(input => input.Rating).InclusiveBetween(1, 10);
        }
    }
}
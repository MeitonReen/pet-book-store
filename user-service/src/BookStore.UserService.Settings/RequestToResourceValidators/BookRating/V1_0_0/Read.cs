using BookStore.UserService.Contracts.BookRating.V1_0_0.Read;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookRating.V1_0_0
{
    public class Read : AbstractValidator<ReadRequest>
    {
        public Read()
        {
            RuleFor(input => input.RatingId).NotEmpty();
        }
    }
}
using BookStore.UserService.Contracts.BookRating.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookRating.V1_0_0
{
    public class Delete : AbstractValidator<DeleteRequest>
    {
        public Delete()
        {
            RuleFor(input => input.RatingId).NotEmpty();
        }
    }
}
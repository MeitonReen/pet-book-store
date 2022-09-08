using BookStore.UserService.Contracts.BookReview.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookReview.V1_0_0
{
    public class Delete : AbstractValidator<DeleteRequest>
    {
        public Delete()
        {
            RuleFor(input => input.ReviewId).NotEmpty();
        }
    }
}
using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyBookReview.V1_0_0
{
    public class DeleteByBook : AbstractValidator<DeleteRequest>
    {
        public DeleteByBook()
        {
            RuleFor(input => input.BookId).NotEmpty();
        }
    }
}
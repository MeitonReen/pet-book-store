using BookStore.UserService.Contracts.MyBookReview.V1_0_0.Update;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyBookReview.V1_0_0;

public class UpdateByBook : AbstractValidator<UpdateRequest>
{
    public UpdateByBook()
    {
        RuleFor(input => input.BookId).NotEmpty();

        RuleFor(input => input.Review)
            .NotEmpty()
            .Length(1000);
    }
}
using BookStore.UserService.Contracts.MyBookRating.V1_0_0.Update;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyBookRating.V1_0_0;

public class UpdateByBook : AbstractValidator<UpdateRequest>
{
    public UpdateByBook()
    {
        RuleFor(input => input.BookId).NotEmpty();

        RuleFor(input => input.Rating).InclusiveBetween(1, 10);
    }
}
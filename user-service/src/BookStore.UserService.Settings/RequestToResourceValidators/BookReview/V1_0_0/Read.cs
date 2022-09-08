using BookStore.UserService.Contracts.BookReview.V1_0_0.Read;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookReview.V1_0_0;

public class Read : AbstractValidator<ReadRequest>
{
    public Read()
    {
        RuleFor(input => input.ReviewId).NotEmpty();
    }
}
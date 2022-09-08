using BookStore.UserService.Contracts.BookRatings.V1_0_0.Read;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.BookRatings.V1_0_0;

public class ReadByBookPart : AbstractValidator<ReadPartRequest>
{
    public ReadByBookPart()
    {
        RuleFor(input => input.BookId).NotEmpty();
        RuleFor(input => input.PartLength).GreaterThan(0);
        RuleFor(input => input.PartNumber).GreaterThan(0);
    }
}
using BookStore.BookService.Contracts.BookCategories.V1_0_0.Read;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.BookCategories.V1_0_0;

public class ReadPart :
    AbstractValidator<ReadPartRequest>
{
    public ReadPart()
    {
        RuleFor(input => input.PartLength).GreaterThan(0);
        RuleFor(input => input.PartNumber).GreaterThan(0);
    }
}
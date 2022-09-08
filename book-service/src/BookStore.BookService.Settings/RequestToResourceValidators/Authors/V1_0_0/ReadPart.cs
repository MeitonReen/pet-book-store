using BookStore.BookService.Contracts.Authors.V1_0_0.Read;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Authors.V1_0_0;

public class ReadPart : AbstractValidator<ReadPartRequest>
{
    public ReadPart()
    {
        RuleFor(input => input.PartLength).GreaterThan(0);
        RuleFor(input => input.PartNumber).GreaterThan(0);
    }
}
using BookStore.BookService.Contracts.BookCategory.V1_0_0.Read;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.BookCategory.V1_0_0;

public class Read : AbstractValidator<ReadRequest>
{
    public Read()
    {
        RuleFor(input => input.CategoryId).NotEmpty();
    }
}
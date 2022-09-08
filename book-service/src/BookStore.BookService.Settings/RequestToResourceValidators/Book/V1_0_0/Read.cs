using BookStore.BookService.Contracts.Book.V1_0_0.Read;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Book.V1_0_0;

public class Read : AbstractValidator<ReadRequest>
{
    public Read()
    {
        RuleFor(input => input.BookId).NotEmpty();
    }
}
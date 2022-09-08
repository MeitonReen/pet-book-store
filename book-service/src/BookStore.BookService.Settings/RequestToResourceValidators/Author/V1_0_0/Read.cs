using BookStore.BookService.Contracts.Author.V1_0_0.Read;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Author.V1_0_0;

public class Read : AbstractValidator<ReadRequest>
{
    public Read()
    {
        RuleFor(input => input.AuthorId).NotEmpty();
    }
}
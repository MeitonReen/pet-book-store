using BookStore.BookService.Contracts.Author.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Author.V1_0_0;

public class Delete : AbstractValidator<DeleteRequest>
{
    public Delete()
    {
        RuleFor(input => input.AuthorId).NotEmpty();
    }
}
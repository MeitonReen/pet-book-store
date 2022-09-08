using BookStore.BookService.Contracts.BookCategory.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.BookCategory.V1_0_0;

public class Delete : AbstractValidator<DeleteRequest>
{
    public Delete()
    {
        RuleFor(input => input.CategoryId).NotEmpty();
    }
}
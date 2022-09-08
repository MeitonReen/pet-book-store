using BookStore.BookService.Contracts.BookCategory.V1_0_0.Update;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.BookCategory.V1_0_0;

public class Update : AbstractValidator<UpdateRequest>
{
    public Update()
    {
        RuleFor(input => input.CategoryId)
            .NotEmpty();

        RuleFor(input => input.Name)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.Description)
            .NotEmpty()
            .Length(1, 2000);
    }
}
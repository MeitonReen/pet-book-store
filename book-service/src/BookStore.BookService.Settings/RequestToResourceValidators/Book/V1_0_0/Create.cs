using BookStore.BookService.Contracts.Book.V1_0_0.Create;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Book.V1_0_0;

public class Create : AbstractValidator<CreateRequest>
{
    public Create()
    {
        RuleFor(input => input.Name)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.Description)
            .NotEmpty()
            .Length(1, 2000);

        RuleFor(input => input.PublicationDate)
            .NotEmpty();
    }
}
using BookStore.BookService.Contracts.Author.V1_0_0.Create;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Author.V1_0_0;

public class Create : AbstractValidator<CreateRequest>
{
    public Create()
    {
        RuleFor(input => input.FirstName)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.LastName)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.Patronymic).Length(1, 500);

        RuleFor(input => input.BirthDate).NotEmpty();
    }
}
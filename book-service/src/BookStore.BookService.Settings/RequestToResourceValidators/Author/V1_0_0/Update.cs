using BookStore.BookService.Contracts.Author.V1_0_0.Update;
using FluentValidation;

namespace BookStore.BookService.Settings.RequestToResourceValidators.Author.V1_0_0;

public class Update : AbstractValidator<UpdateRequest>
{
    public Update()
    {
        RuleFor(input => input.AuthorId).NotEmpty();

        RuleFor(input => input.FirstName)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.LastName)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.Patronymic)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.BirthDate).NotEmpty();
    }
}
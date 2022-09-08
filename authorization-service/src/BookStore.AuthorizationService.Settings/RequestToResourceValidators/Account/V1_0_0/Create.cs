using BookStore.AuthorizationService.Contracts.Account.V1_0_0.Create;
using FluentValidation;

namespace BookStore.AuthorizationService.Settings.RequestToResourceValidators.Account.V1_0_0;

public class Create : AbstractValidator<CreateRequest>
{
    public Create()
    {
        RuleFor(input => input.Login)
            .NotEmpty()
            .Length(1, 500);

        RuleFor(input => input.Password)
            .NotEmpty()
            .Length(1, 500);
    }
}
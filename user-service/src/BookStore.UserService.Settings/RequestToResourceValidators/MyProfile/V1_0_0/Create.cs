using BookStore.UserService.Contracts.MyProfile.V1_0_0.Create;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.MyProfile.V1_0_0
{
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

            RuleFor(input => input.Patronymic)
                .NotEmpty()
                .Length(1, 500);
        }
    }
}
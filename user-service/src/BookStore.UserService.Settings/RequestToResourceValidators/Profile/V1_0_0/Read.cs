using BookStore.UserService.Contracts.Profile.V1_0_0.Read;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.Profile.V1_0_0
{
    public class Read : AbstractValidator<ReadRequest>
    {
        public Read()
        {
            RuleFor(input => input.UserId).NotEmpty();
        }
    }
}
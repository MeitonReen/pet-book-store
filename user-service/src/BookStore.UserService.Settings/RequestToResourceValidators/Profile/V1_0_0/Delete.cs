using BookStore.UserService.Contracts.Profile.V1_0_0.Delete;
using FluentValidation;

namespace BookStore.UserService.Settings.RequestToResourceValidators.Profile.V1_0_0
{
    public class Delete : AbstractValidator<DeleteRequest>
    {
        public Delete()
        {
            RuleFor(input => input.UserId).NotEmpty();
        }
    }
}
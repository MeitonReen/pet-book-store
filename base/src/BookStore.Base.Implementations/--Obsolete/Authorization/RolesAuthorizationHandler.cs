using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace BookStore.Base.Implementations.__Obsolete.Authorization
{
    public class RolesAuthorizationHandler : AuthorizationHandler<RolesRequirementOr>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
            RolesRequirementOr requirement)
        {
            if (!context.User.HasClaim(claim =>
                    claim.Type == OpenIddictConstants.Claims.Role && requirement.Roles.Contains(claim.Value)))
            {
                return Task.CompletedTask;
            }

            ;

            context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}
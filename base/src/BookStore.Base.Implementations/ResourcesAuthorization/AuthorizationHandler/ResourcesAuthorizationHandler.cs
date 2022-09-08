using BookStore.Base.Implementations.ResourcesAuthorization.Helpers;
using BookStore.Base.Implementations.ResourcesAuthorization.Requirement;
using Microsoft.AspNetCore.Authorization;
using OpenIddict.Abstractions;

namespace BookStore.Base.Implementations.ResourcesAuthorization.AuthorizationHandler;

public class ResourcesAuthorizationHandler : AuthorizationHandler<ResourcesRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
        ResourcesRequirement requirement)
    {
        var userScopeSet = context.User.Claims
            .Where(claim => claim.Type == OpenIddictConstants.Claims.Private.Scope)
            .Select(claim => claim.Value)
            .ToArray();

        var result = requirement.AndSetsByOrResourcesMdnf
            .FirstOrDefault(requirementAndResourceSet => requirementAndResourceSet
                .All(requirementAndResource => userScopeSet
                        .FirstOrDefault(userScope => ScopeHelper
                            .FirstScopeIncludesSecondScope(userScope, requirementAndResource)
                        ) != default
                ));

        if (result == default) return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}
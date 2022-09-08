using Microsoft.AspNetCore.Authorization;

namespace BookStore.Base.Implementations.ResourcesAuthorization.Requirement.Provider.Abstrations;

public interface IResourcesRequirementPolicyProvider
{
    IAuthorizationRequirement Provide(string mdnfResources);
    IAuthorizationRequirement Provide(string[][] mdnfResourcesAsAndSetsByOrResources);
}
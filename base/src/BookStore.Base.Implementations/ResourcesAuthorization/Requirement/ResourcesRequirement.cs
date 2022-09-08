using BookStore.Base.Implementations.ResourcesAuthorization.Contracts;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Base.Implementations.ResourcesAuthorization.Requirement;

public class ResourcesRequirement : IAuthorizationRequirement
{
    public ResourcesRequirement(string[][] andSetsByOrResourcesMdnf)
    {
        AndSetsByOrResourcesMdnf = andSetsByOrResourcesMdnf;
    }

    public ResourcesRequirement(string mdnf) =>
        AndSetsByOrResourcesMdnf = mdnf
            .Split(ResourcesPolicyConstants.Or)
            .Select(andResources => andResources.Split(ResourcesPolicyConstants.And))
            .ToArray();

    public string[][] AndSetsByOrResourcesMdnf { get; init; }
}
using BookStore.Base.Implementations.ResourcesAuthorization.Contracts;
using BookStore.Base.Implementations.ResourcesAuthorization.Requirement.Provider.Abstrations;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Base.Implementations.ResourcesAuthorization.Requirement.Provider.Implementations;

public class ResourcesRequirementPolicyProvider : IResourcesRequirementPolicyProvider
{
    private readonly List<(string[][] SortedAndSetsByOrResources, Guid CachedRequirementId)> _mdnfCache = new();
    private readonly Dictionary<Guid, IAuthorizationRequirement> _requirementCache = new();

    public IAuthorizationRequirement Provide(string mdnfResources) =>
        Provide(mdnfResources
            .Split(ResourcesPolicyConstants.Or)
            .Select(andResources => andResources.Split(ResourcesPolicyConstants.And))
            .ToArray());

    public IAuthorizationRequirement Provide(string[][] mdnfResourcesAsAndSetsByOrResources)
    {
        var inputtedSortedAndByOrResources = mdnfResourcesAsAndSetsByOrResources
            .Select(andSet => andSet
                .OrderBy(andSetEl => andSetEl)
                .ToArray())
            .ToArray();

        if (!_mdnfCache.Any()) return CreateNewRequirement(inputtedSortedAndByOrResources);

        var match = Match(inputtedSortedAndByOrResources);

        return match ?? CreateNewRequirement(inputtedSortedAndByOrResources);
    }

    private IAuthorizationRequirement? Match(string[][] inputSortedAndByOrResources)
    {
        var match = _mdnfCache
            .FirstOrDefault(mdnf => inputSortedAndByOrResources
                .All(inputAndSet => mdnf.SortedAndSetsByOrResources
                    .FirstOrDefault(inputAndSet.SequenceEqual) != default));

        return match == default ? default : _requirementCache[match.CachedRequirementId];
    }

    private IAuthorizationRequirement CreateNewRequirement(string[][] sortedAndByOrResources)
    {
        var cacheId = Guid.NewGuid();
        var requirement = new ResourcesRequirement(sortedAndByOrResources);

        _mdnfCache.Add((sortedAndByOrResources, cacheId));
        _requirementCache.Add(cacheId, requirement);

        return requirement;
    }
}
using BookStore.Base.NonAttachedExtensions;

namespace BookStore.Base.Implementations.ResourcesAuthorization.Helpers;

public static class ScopeHelper
{
    public static bool FirstScopeIncludesSecondScope(string firstScope, string secondScope)
    {
        var firstScopeParts = firstScope.Split('.');
        var secondScopeParts = secondScope.Split('.');

        if (firstScopeParts.Length == 1 && secondScopeParts.Length == 1)
        {
            return firstScope == secondScope;
        }

        if (firstScopeParts.Length == 1 || secondScopeParts.Length == 1) return false;

        var (firstScopeResource, firstScopePermissions) = firstScopeParts;
        var (secondScopeResource, secondScopePermissions) = secondScopeParts;

        return firstScopeResource == secondScopeResource &&
               secondScopePermissions.All(el => firstScopePermissions.Contains(el));
    }
}
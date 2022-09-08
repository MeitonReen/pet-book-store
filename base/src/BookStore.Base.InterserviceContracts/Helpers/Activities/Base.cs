using BookStore.Base.NonAttachedExtensions;

namespace BookStore.Base.InterserviceContracts.Helpers.Activities;

public static class Base
{
    public static string GetActivityNameKebabCaseFromNamespace(string @namespace)
        => @namespace.ToKebabCaseNameFromNamespace()
           + Constants.ExecuteActivityEndpointNameDefaultPostfix;

    public static string GetActivityNameKebabCaseFromTypeFullName(string typeFullName)
        => typeFullName.ToKebabCaseNameFromTypeFullName()
           + Constants.ExecuteActivityEndpointNameDefaultPostfix;
}
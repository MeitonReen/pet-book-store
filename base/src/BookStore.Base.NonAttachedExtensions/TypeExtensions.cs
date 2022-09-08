using System.Reflection;

namespace BookStore.Base.NonAttachedExtensions;

public static class TypeExtensions
{
    public static string GetAbsoluteFilePathFromNamespace(this Type targetAssemblyType)
    {
        var assemblyName = targetAssemblyType.Assembly.GetName().Name;

        var localFilePathFromTargetAssemblyType = targetAssemblyType
            .Namespace?
            .Replace($"{assemblyName}.", "")
            .Replace('.', '/');

        if (localFilePathFromTargetAssemblyType == default)
        {
            //logger?.LogWarning($"Namespace from {targetAssemblyType.Name} does not point at appSettings file path. {targetAssemblyType.Name} is in the same folder as appSettings.json?");
            return string.Empty;
        }

        var execAssemblyLocation = Assembly.GetExecutingAssembly().Location;

        var absolutePathToExecAssembly = Path
            .TrimEndingDirectorySeparator(execAssemblyLocation
                .Replace(Path.GetFileName(execAssemblyLocation), ""));

        return Path.Combine(absolutePathToExecAssembly, localFilePathFromTargetAssemblyType);
    }
}
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace BookStore.Base.Implementations.OpenApi;

public class GroupingInSpecsByApiVersions : IControllerModelConvention
{
    private const string SemanticVersionRegexPattern =
        @"^V([0-9]+)_([0-9]+)_([0-9]+)(?:-([0-9A-Za-z-]+(?:_[0-9A-Za-z-]+)*))?(?:\+[0-9A-Za-z-]+)?$";

    public void Apply(ControllerModel controller)
    {
        var controllerNamespace = controller.ControllerType.Namespace;
        if (controllerNamespace == default)
            throw new InvalidOperationException($"{nameof(controllerNamespace)} is default");

        var apiVersionMatch = new Regex(SemanticVersionRegexPattern).Match(controllerNamespace);

        if (!apiVersionMatch.Success) return;

        var apiVersion = apiVersionMatch.Value
            .Replace('_', '.')
            .ToLowerInvariant();

        controller.ApiExplorer.GroupName = apiVersion;
    }
}
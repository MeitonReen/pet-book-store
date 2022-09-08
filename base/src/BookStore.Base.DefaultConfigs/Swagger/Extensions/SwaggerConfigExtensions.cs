using System.Reflection;
using BookStore.Base.Abstractions;

namespace BookStore.Base.DefaultConfigs.Swagger.Extensions;

public static class SwaggerConfigExtensions
{
    public static SwaggerConfig LoadOpenApiDocConfigs(this SwaggerConfig swaggerConfig,
        Assembly apiRouteBuildersAssembly)
    {
        const string version = "Version";

        var apiVersions = apiRouteBuildersAssembly
            .GetTypes()
            .Where(type => typeof(HttpApiRouteBuilderBase).IsAssignableFrom(type))
            .Select(type => type
                                .GetField(version)
                                ?.GetRawConstantValue() as string ??
                            throw new InvalidOperationException(
                                $"{version} in ApiRouteBuilder is not found"))
            .Distinct();
        ;

        var serviceGroupNameWithServiceName = GetTwoFirstPartProjectName(apiRouteBuildersAssembly);

        swaggerConfig.OpenApiDocs = apiVersions
            .Select(el =>
            {
                var docName = $"{serviceGroupNameWithServiceName} {el}";
                return new OpenApiDocConfig
                {
                    Version = el.ToLowerInvariant(),
                    BasePath = HttpApiRouteBuilderBase.Base,
                    Name = docName,
                    Title = docName
                };
            })
            .ToArray();

        return swaggerConfig;
    }

    /// <summary>
    ///--&gt;[BookStore.BookService.SomeOtherPart...]
    ///&lt;--[BookStore.BookService]
    /// </summary>
    private static string GetTwoFirstPartProjectName(Assembly targetApiRoutesAssembly)
    {
        var execAssemblyName = targetApiRoutesAssembly.GetName().Name;
        if (execAssemblyName == default)
            throw new InvalidOperationException("Get exec assembly name error");

        var twoDotIndexInProjectName = execAssemblyName.IndexOf('.',
            execAssemblyName.IndexOf('.') + 1);

        if (twoDotIndexInProjectName == default)
            throw new InvalidOperationException(
                $"{nameof(twoDotIndexInProjectName)} not found");

        return execAssemblyName[..twoDotIndexInProjectName];
    }
}
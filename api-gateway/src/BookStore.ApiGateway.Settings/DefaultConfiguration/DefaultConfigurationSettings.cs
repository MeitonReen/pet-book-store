using System.Text;
using System.Text.RegularExpressions;
using BookStore.ApiGateway.Configs;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.Extensions.Configuration;

namespace BookStore.ApiGateway.Settings.DefaultConfiguration;

public static class DefaultConfigurationSettings
{
    public static IConfiguration ApplyDefaultApiGatewaySettings(
        this IConfiguration generalConfig)
    {
        var appConfig = generalConfig.Get<AppConfig>(sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        generalConfig["GlobalConfiguration:BaseUrl"] = appConfig.ExternalUri;

        const string authorizationServiceEndpointKey = "book-store.authorization-service";
        const string bookServiceEndpointKey = "book-store.book-service";
        const string userServiceEndpointKey = "book-store.user-service";
        const string orderServiceEndpointKey = "book-store.order-service";

        var endpointKeyToOpenApiConfigUriMap = new Dictionary<string, string>
        {
            [authorizationServiceEndpointKey] = appConfig.AuthorizationServiceUri,
            [bookServiceEndpointKey] = appConfig.BookServiceUri,
            [userServiceEndpointKey] = appConfig.UserServiceUri,
            [orderServiceEndpointKey] = appConfig.OrderServiceUri
        };

        SetUrlsToSwaggerEndpointsConfig(generalConfig, endpointKeyToOpenApiConfigUriMap);

        SetHostAndPortToOcelotDownstreamConfig(generalConfig, endpointKeyToOpenApiConfigUriMap);

        // ParseData(generalConfig);

        return generalConfig;
    }


    private static void ParseData(IConfiguration generalConfig)
    {
        var routes = generalConfig
            .GetSection("Routes")
            .Get<Route[]>();

        var httpCrudToDefaultCrudMap = new Dictionary<string, string>
        {
            ["Post"] = "C",
            ["Get"] = "R",
            ["Patch"] = "U",
            ["Delete"] = "D"
        };
        /*
         * path:
         * C-->Post-->Description
         */

        var result = routes
            .Aggregate(new StringBuilder(), (aggregator, el) =>
            {
                aggregator.Append($"\n{el.UpstreamPathTemplate}:");

                aggregator = el.UpstreamHttpMethod.Aggregate(aggregator,
                    (aggregatorInner, elHttpMethod) =>
                    {
                        aggregatorInner.Append(
                            $"\n* {httpCrudToDefaultCrudMap[elHttpMethod]}"
                            + $"-->{elHttpMethod}-->Description");
                        return aggregatorInner;
                    });
                return aggregator;
            })
            .ToString();
        ;
    }

    /// <summary>
    /// OpenApiConfigUri is match with downstream host and port
    /// </summary>
    private static IConfiguration SetHostAndPortToOcelotDownstreamConfig(
        IConfiguration generalConfig,
        IReadOnlyDictionary<string, string> endpointKeyToOpenApiConfigUriMap)
    {
        const string routesConfigToken = "Routes";

        const string hostRegexGroup = "HostGroup";
        const string portRegexGroup = "PortGroup";

        const string hostAndPortRegex =
            @"(?<=https?://)"
            + $@"(?<{hostRegexGroup}>"
            + @"([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9])(\.([a-zA-Z0-9]|[a-zA-Z0-9][a-zA-Z0-9\-]{0,61}[a-zA-Z0-9]))*)"
            + @"("
            + @"(:"
            + $@"(?<{portRegexGroup}>"
            + @"[1-9]\d{0,3}"
            + @"|[1-5]\d{4}"
            + @"|6[0-4]\d{3}"
            + @"|65[0-4]\d{2}"
            + @"|655[0-2]\d"
            + @"|6553[0-5])"
            + @"\b)"
            + @"|\b)";

        var endpointKeyToHostAndPortMap = endpointKeyToOpenApiConfigUriMap
            .Aggregate(new Dictionary<string, (string Host, string Port)>(),
                (accumulator, el) =>
                {
                    accumulator.Add(el.Key, GetHostAndPort(el.Value, hostAndPortRegex,
                        hostRegexGroup, portRegexGroup));
                    return accumulator;
                });

        var swaggerKeysFromRoutes = generalConfig
            .GetSection(routesConfigToken)
            .Get<TargetPartFromRoute[]>();

        _ = swaggerKeysFromRoutes
            .Select((route, i1) => route.DownstreamHostAndPorts
                .Select((_, i2) =>
                {
                    var downstreamHostAndPortsPicker =
                        $"{routesConfigToken}:{i1}:DownstreamHostAndPorts:{i2}";

                    generalConfig[$"{downstreamHostAndPortsPicker}:Host"]
                        = endpointKeyToHostAndPortMap[route.SwaggerKey].Host;
                    generalConfig[$"{downstreamHostAndPortsPicker}:Port"]
                        = endpointKeyToHostAndPortMap[route.SwaggerKey].Port;

                    return true;
                })
                .ToArray())
            .ToArray();

        return generalConfig;
    }

    private static (string Host, string Port) GetHostAndPort(string targetValue,
        string hostAndPortRegex, string hostRegexGroupName, string portRegexGroupName)
    {
        var res = Regex.Match(targetValue, hostAndPortRegex);
        if (!res.Groups[hostRegexGroupName].Success)
            throw new InvalidOperationException(
                $"Host are not contained in {targetValue}");

        var host = res.Groups[hostRegexGroupName].Value;

        if (host.Length > 255)
            throw new InvalidOperationException("Host lenght greater 255");

        string port;

        if (res.Groups[portRegexGroupName].Success)
        {
            port = res.Groups[portRegexGroupName].Value;
        }
        else
        {
            port = targetValue.Contains("https") ? "443" : "80";
        }

        return (host, port);
    }
    /*
         "Routes": [
        {
          "UpstreamPathTemplate": "/api/book-rating/v1.0.0",
          "UpstreamHttpMethod": [
            "Get",
            "Patch",
            "Delete"
          ],
          "DownstreamPathTemplate": "/api/book-rating/v1.0.0",
          "DownstreamScheme": "http",
          "DownstreamHostAndPorts": [
            {
              "Host": "<Host and port from environment>",
              "Port": 7187
            }
          ],
          "SwaggerKey": "book-store.user-service"
        }
      ]
    */

    private static IConfiguration SetUrlsToSwaggerEndpointsConfig(
        IConfiguration generalConfig,
        IReadOnlyDictionary<string, string> endpointKeyToOpenApiConfigUriMap)
    {
        const string swaggerEndpointsConfigToken = "SwaggerEndPoints";

        var swaggerForOcelotEndpoints = generalConfig
            .GetSection(swaggerEndpointsConfigToken)
            .Get<SwaggerForOcelotEndpoint[]>();

        _ = swaggerForOcelotEndpoints
            .Select((endpoint, i1) => endpoint.Config
                .Select((endpointConfig, i2) =>
                {
                    generalConfig[$"{swaggerEndpointsConfigToken}:{i1}:Config:{i2}:Url"]
                        = $"{endpointKeyToOpenApiConfigUriMap[endpoint.Key]}/swagger/{endpointConfig.Version}/swagger.json";
                    return true;
                })
                .ToArray())
            .ToArray();

        return generalConfig;
    }

    private class Route
    {
        public string UpstreamPathTemplate { get; set; }
        public string[] UpstreamHttpMethod { get; set; }
    }

    private class TargetPartFromRoute
    {
        public string SwaggerKey { get; set; } = string.Empty;
        public DownstreamHostAndPort[] DownstreamHostAndPorts { get; set; } = Array.Empty<DownstreamHostAndPort>();
    }

    private class DownstreamHostAndPort
    {
    }

    private class SwaggerForOcelotEndpoint
    {
        public string Key { get; set; } = string.Empty;

        public SwaggerForOcelotEndpointConfig[] Config { get; set; }
            = Array.Empty<SwaggerForOcelotEndpointConfig>();
    }

    private class SwaggerForOcelotEndpointConfig
    {
        public string Version { get; set; } = string.Empty;
    }
    // {
    //     "Key": "book-store.book-service",
    //     "Config": [
    //     {
    //         "Name": "BookStore.BookService",
    //         "Version": "v1.0.0",
    //         "Url": "<FromEnvironment>"
    //     }
    //     ]
    // },
}
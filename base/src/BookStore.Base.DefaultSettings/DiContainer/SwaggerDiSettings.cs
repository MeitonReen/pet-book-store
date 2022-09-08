using System.Reflection;
using BookStore.AuthorizationService.Defaults;
using BookStore.Base.Abstractions;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultConfigs.Swagger;
using BookStore.Base.DefaultConfigs.Swagger.Extensions;
using BookStore.Base.Implementations.OpenApi;
using BookStore.Base.Implementations.OpenApi.OperationFilters;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using OpenIddict.Abstractions;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace BookStore.Base.DefaultSettings.DiContainer;

public static class SwaggerDiSettings
{
    public static IServiceCollection AddDefaultSwaggerSettings(
        this IServiceCollection services,
        AuthorizationServiceConfig authorizationServiceConfig,
        SwaggerConfig swaggerConfig,
        string xmlDocFileName,
        string xmlDocPath,
        Assembly apiRouteBuilderAssembly)
    {
        swaggerConfig = swaggerConfig.LoadOpenApiDocConfigs(apiRouteBuilderAssembly);

        return services
            .Configure<SwaggerUIOptions>(sets =>
            {
                sets.OAuthClientId(swaggerConfig.Ui.ClientId);
                sets.OAuthClientSecret(swaggerConfig.Ui.ClientSecret);

                Array.ForEach(swaggerConfig.OpenApiDocs, el =>
                    sets.SwaggerEndpoint($"/swagger/{el.Version}/swagger.json", el.Name));
                //conf.UseRequestInterceptor("(req) => { if (req.url.endsWith('connect/token') && req.body) req.body += '&audience=" + Constants.Authentication.AudienceSelector.BookStore.BookServiceApi.Select + "'; return req; }");
            })
            .AddMvc(sets => sets.Conventions.Add(new GroupingInSpecsByApiVersions()))
            .Services
            .AddSwaggerGen(sets => sets.DefaultSwaggerSettings(authorizationServiceConfig,
                swaggerConfig.OpenApiDocs, xmlDocFileName, xmlDocPath));
    }

    private static SwaggerGenOptions DefaultSwaggerSettings(
        this SwaggerGenOptions sets,
        AuthorizationServiceConfig authorizationServiceConfig,
        OpenApiDocConfig[] openApiDocConfigs,
        string xmlDocFileName,
        string xmlDocPath
    )
    {
        sets.UseDateOnlyTimeOnlyStringConverters(); //Temp DateOnly support

        Array.ForEach(openApiDocConfigs, el =>
        {
            sets.SwaggerDoc(el.Version,
                new OpenApiInfo {Title = el.Title, Version = el.Version});
        });

        var fullPathToDocFile = Path.Combine(xmlDocPath, xmlDocFileName);

        sets.IncludeXmlComments(fullPathToDocFile);

        sets.CustomSchemaIds(schemaType => schemaType.FullName);

        sets.CustomOperationIds(methodDescription =>
        {
            //"--> displayActionName == [<A.B.C.D.E.CreateResource> (<Assembly>)]"
            //<-- operationId == createResource
            var displayActionName = methodDescription.ActionDescriptor.DisplayName;
            var firstPartDisplayName = displayActionName?[..displayActionName.LastIndexOf(' ')];
            var actionName = firstPartDisplayName?[(firstPartDisplayName.LastIndexOf('.') + 1)..];

            if (actionName == default)
            {
                throw new InvalidOperationException($"{nameof(actionName)} is invalid");
            }

            var operationId = char.ToLowerInvariant(actionName[0]) + actionName[1..];

            return operationId;
        });

        //Groupoing by root resource
        //Examples:
        //service/api/v1/resource
        //<-- Resource
        //service/api/v1/resource1/resource2/{resourceId}
        //<-- Resource1
        sets.TagActionsBy(api =>
        {
            //--> api.RelativePath == [<api/v1/rootResource/...>]
            //<-- []{ rootResource }
            const string basePath = HttpApiRouteBuilderBase.Base;

            var fromRootResourceToEndPath = api.RelativePath?.Replace($"{basePath}/", "");

            if (fromRootResourceToEndPath == default)
            {
                throw new InvalidOperationException(
                    $"{nameof(fromRootResourceToEndPath)} is invalid");
            }

            var rootResourceEndSymbolNumber = fromRootResourceToEndPath.IndexOf('/');
            rootResourceEndSymbolNumber = rootResourceEndSymbolNumber != -1
                ? rootResourceEndSymbolNumber
                : fromRootResourceToEndPath.Length;

            var rootResource = fromRootResourceToEndPath[..rootResourceEndSymbolNumber];

            rootResource = char.ToUpperInvariant(rootResource[0]) + rootResource[1..];

            return new[] {rootResource};
        });

        sets.OperationFilter<OpenApiSetFromBodyContentType>();
        sets.OperationFilter<OpenAPIOperationsSecurityFilter>();
        
        sets.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
        sets.OperationFilter<SecurityRequirementsOperationFilter>();
        
        sets.AddSecurityDefinition(OpenIddictConstants.Schemes.Bearer,
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri(authorizationServiceConfig.SignInUri),
                        TokenUrl = new Uri(authorizationServiceConfig.TokenUri),
                        Scopes = typeof(BookStoreDefaultScopes)
                                     .GetFields(BindingFlags.Public | BindingFlags.Static)
                                     .Select(el => el.GetValue(default) as string ??
                                                   throw new InvalidOperationException(
                                                       "Specified scope not found"))
                                     .Append(OpenIddictConstants.Scopes.OpenId)
                                     .Append(OpenIddictConstants.Scopes.Profile)
                                     .ToDictionary(scopeName => scopeName, _ => "") ??
                                 throw new InvalidOperationException("Specified scopes not found")
                    }
                }
            });
        sets.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = OpenIddictConstants.Schemes.Bearer
                    },
                    Scheme = OpenIddictConstants.Schemes.Bearer,
                    Name = OpenIddictConstants.Schemes.Bearer,
                    In = ParameterLocation.Header
                },
                new List<string>()
            }
        });
        return sets;
    }
}
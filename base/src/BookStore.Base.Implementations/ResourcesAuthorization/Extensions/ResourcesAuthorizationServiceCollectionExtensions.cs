using System.Reflection;
using BookStore.Base.Implementations.MinimalDnf;
using BookStore.Base.Implementations.ResourcesAuthorization.AuthorizationHandler;
using BookStore.Base.Implementations.ResourcesAuthorization.Contracts;
using BookStore.Base.Implementations.ResourcesAuthorization.Requirement.Provider.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace BookStore.Base.Implementations.ResourcesAuthorization.
    Extensions;

public static class ResourcesAuthorizationServiceCollectionExtensions
{
    public static async Task<IServiceCollection> AddResourcesAuthorization(
        this IServiceCollection services,
        Assembly targetControllersAssembly)
    {
        var actionMethods = targetControllersAssembly
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(
                BindingFlags.Instance
                | BindingFlags.DeclaredOnly
                | BindingFlags.Public))
            .Where(method => !method.IsDefined(typeof(NonActionAttribute))
                             && !method.IsDefined(typeof(AllowAnonymousAttribute))
                             && method.IsDefined(typeof(AuthorizeAttribute)));
        ;

        var definedPolicyStrings = actionMethods
            .Select(actionMethod =>
            {
                try
                {
                    return actionMethod.CustomAttributes.Single(attrData =>
                        attrData.AttributeType == typeof(AuthorizeAttribute));
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(
                        $"Multiple authorize attribute is denied: {actionMethod.Name}");
                }
            })
            .Select(authorizeAttribute =>
            {
                if (authorizeAttribute
                        .ConstructorArguments
                        .SingleOrDefault()
                        .Value is string policyStringFromConstructorArgs)
                    return policyStringFromConstructorArgs;

                if (authorizeAttribute
                        .NamedArguments
                        .SingleOrDefault(arg => arg.MemberName == nameof(
                            AuthorizeAttribute.Policy))
                        .TypedValue.Value is string policyStringFromNamedArg)
                    return policyStringFromNamedArg;

                throw new InvalidOperationException("Policy name not found");
            });
        var resourcesRequirementProvider = new ResourcesRequirementPolicyProvider();


        var definedPolicyStringsByAuthorizationRequirement = await Task
            .WhenAll(definedPolicyStrings
                .Select(async el => (
                    DefinedPolicyString: el,
                    AuthorizationRequirement: resourcesRequirementProvider
                        .Provide(await new AbbreviatedMinimalDnfGenerator(
                                ResourcesPolicyConstants.And,
                                ResourcesPolicyConstants.Or,
                                ResourcesPolicyConstants.ValueRegexSelector)
                            .Generate(el))
                )));

        services.AddAuthorization(authrSets => Array
            .ForEach(definedPolicyStringsByAuthorizationRequirement,
                el => authrSets
                    .AddPolicy(el.DefinedPolicyString, policySets => policySets
                        .RequireAuthenticatedUser()
                        .RequireClaim(OpenIddictConstants.Claims.Subject)
                        .AddRequirements(el.AuthorizationRequirement))
            ));

        services.AddSingleton<IAuthorizationHandler, ResourcesAuthorizationHandler>();

        return services;
    }
}
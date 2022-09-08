using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.DefaultSettings.DiContainer;

public static class FluentValidationDiSettings
{
    public static IServiceCollection AddDefaultFluentValidationSettings(
        this IServiceCollection services,
        Assembly validatorsAssembly)
        => services
            .AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssembly(validatorsAssembly);
}
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.Result;
using BookStore.Base.Implementations.Result.Builders.Exception.Extensions;
using BookStore.Base.Implementations.Result.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseBookStoreDefaultExceptionHandling(
        this IApplicationBuilder applicationBuilder)
        => applicationBuilder.UseExceptionHandler(new ExceptionHandlerOptions
        {
            ExceptionHandler = async httpContext =>
            {
                var appConfig = httpContext.RequestServices
                    .GetRequiredService<IOptionsSnapshotMixOptionsMonitor<AppConfig>>()
                    .Value;

                var actionContext = new ActionContext(httpContext, new RouteData(),
                    new ActionDescriptor());
                var actionResult = ResultModelBuilder
                    .Exception()
                    .ApplyDefaultSettings(
                        "The exception occurred outside of the action and filters")
                    .Environment(appConfig.Environment)
                    .Build()
                    .ToActionResult();

                await actionResult.ExecuteResultAsync(actionContext);
            },
            ExceptionHandlingPath = default,
            AllowStatusCode404Response = false
        });
}
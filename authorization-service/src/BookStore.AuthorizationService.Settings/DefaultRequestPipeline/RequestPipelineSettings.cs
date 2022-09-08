using BookStore.AuthorizationService.Configs;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;
using BookStore.Base.Implementations.CorrelationId.Middleware.Extensions;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BookStore.AuthorizationService.Settings.DefaultRequestPipeline;

public static class RequestPipelineSettings
{
    public static WebApplication UseRequestPipelineSettings(this WebApplication
        webApplication, IConfiguration configuration)
    {
        var appConfig = AppConfig.Empty;
        if (webApplication.Environment.IsDevelopment())
        {
            appConfig = configuration.Get<AppConfig>(conf =>
            {
                conf.CasesSupport.AddUpperSnakeCase();
                conf.IncludeRootConfigClassAsConfigProperty = true;
            });
        }

        webApplication.UseSerilogRequestLogging();
        webApplication.UseHttpCorrelationId();

        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();
        }

        webApplication.UseSpaStaticFiles();

        webApplication.UseRouting();
        webApplication.UseCors();

        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
        webApplication.MapControllers();

        webApplication.UseBookStoreDefaultExceptionHandling();

        webApplication.UseEndpoints(_ => { });

        webApplication.UseSpa(_ => { }
            // {
            //     if (webApplication.Environment.IsDevelopment())
            //     {
            //         sets.UseProxyToSpaDevelopmentServer(appConfig.SpaDevelopmentServerUri);
            //     }
            // }
        );

        return webApplication;
    }
}

public static class HostEnvironmentExtensions
{
    private const string LowDevelopment = nameof(LowDevelopment);

    public static bool IsLowDevelopment(
        this IHostEnvironment hostEnvironment)
    {
        if (hostEnvironment == null)
        {
            throw new ArgumentNullException(nameof(hostEnvironment));
        }

        return string.Equals(
            hostEnvironment.EnvironmentName,
            LowDevelopment,
            StringComparison.OrdinalIgnoreCase);
    }
}
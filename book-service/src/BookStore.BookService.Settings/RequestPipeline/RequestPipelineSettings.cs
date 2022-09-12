using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;
using BookStore.Base.Implementations.CorrelationId.Middleware.Extensions;
using BookStore.Base.Implementations.UserClaimsProfile.Middleware.Default.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BookStore.BookService.Settings.RequestPipeline;

public static class RequestPipelineSettings
{
    public static WebApplication UseRequestPipelineSettings(this WebApplication
        webApplication)
    {
        webApplication.UseHealthChecks("/healthz");
        
        webApplication.UseSerilogRequestLogging();

        webApplication.UseHttpCorrelationId();

        if (webApplication.Environment.IsDevelopment())
        {
            webApplication.UseSwagger();
            webApplication.UseSwaggerUI();
        }

        webApplication.UseAuthentication();
        webApplication.UseUserClaimsProfile();

        webApplication.UseAuthorization();

        webApplication.UseBookStoreDefaultExceptionHandling();

        webApplication.MapControllers();
        // webApplication.MapHealthChecks("/healthz");

        return webApplication;
    }
}
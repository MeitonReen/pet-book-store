using Microsoft.AspNetCore.Builder;
using Ocelot.Middleware;

namespace BookStore.ApiGateway.Settings.RequestPipeline;

public static class RequestPipelineSettings
{
    public static Task<IApplicationBuilder> UseRequestPipelineSettings(this IApplicationBuilder
        applicationBuilder)
        => applicationBuilder
            .UseHealthChecks("/healthz")
            .UseSwaggerForOcelotUI()
            .UseOcelot();
}
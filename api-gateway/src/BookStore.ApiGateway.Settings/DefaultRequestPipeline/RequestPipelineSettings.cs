using Microsoft.AspNetCore.Builder;
using Ocelot.Middleware;

namespace BookStore.ApiGateway.Settings.DefaultRequestPipeline;

public static class RequestPipelineSettings
{
    public static Task<IApplicationBuilder> UseRequestPipelineSettings(this IApplicationBuilder
        applicationBuilder)
        => applicationBuilder
            .UseSwaggerForOcelotUI()
            .UseOcelot();
}
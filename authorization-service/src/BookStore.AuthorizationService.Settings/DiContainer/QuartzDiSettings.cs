using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace BookStore.AuthorizationService.Settings.DiContainer;

public static class QuartzDiSettings
{
    public static IServiceCollection AddQuartzSettings(this IServiceCollection services)
        => services
            .AddQuartz(options =>
            {
                options.UseMicrosoftDependencyInjectionJobFactory();
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            })
            .AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
}
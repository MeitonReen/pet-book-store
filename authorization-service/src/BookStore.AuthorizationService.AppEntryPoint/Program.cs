using BookStore.AuthorizationService.Settings.DefaultAppSettings;
using BookStore.AuthorizationService.Settings.DefaultRequestPipeline;
using BookStore.AuthorizationService.Settings.DiContainer;
using BookStore.Base.DefaultSettings.ConfigurationManager;
using BookStore.Base.DefaultSettings.Logger;
using BookStore.Base.Implementations.DatabaseInit;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Configuration
    .ApplyDefaultSettings(typeof(TargetAssemblyType))
    .ApplyDefaultLoggerSettings(appBuilder.Host);

await appBuilder.Services.AddDiSettings(appBuilder.Configuration);

var app = appBuilder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await InitializeData(app.Services);

app.UseRequestPipelineSettings(app.Configuration);

app.Run();

LoggerDefaultSettings.CloseAndFlush();

async Task InitializeData(IServiceProvider baseServiceProvider)
{
    using var scope = baseServiceProvider.CreateScope();

    var targetScopeServiceProvider = scope.ServiceProvider;
    var dataInitializers = targetScopeServiceProvider.GetServices<IDatabaseInit>();

    await dataInitializers
        .ToAsyncEnumerable()
        .ForEachAwaitAsync(async initer => await initer.SeedAsync());
}
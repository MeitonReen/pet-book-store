using BookStore.ApiGateway.Settings.DefaultAppSettings;
using BookStore.ApiGateway.Settings.DefaultConfiguration;
using BookStore.ApiGateway.Settings.DefaultRequestPipeline;
using BookStore.ApiGateway.Settings.DiContainer;
using BookStore.Base.DefaultSettings.ConfigurationManager;
using BookStore.Base.DefaultSettings.Logger;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Serilog;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Configuration
    .ApplyDefaultSettings(typeof(TargetAssemblyType), (configManager, filePath) => configManager
        .AddOcelotWithSwaggerSupport(sets => sets.Folder = filePath))
    .ApplyDefaultLoggerSettings(appBuilder.Host)
    .ApplyDefaultApiGatewaySettings();

Log.Information(appBuilder.Configuration.GetDebugView());
;

appBuilder.Services.AddDiSettings(appBuilder.Configuration);

var app = appBuilder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

app.UseRequestPipelineSettings();

app.Run();

LoggerDefaultSettings.CloseAndFlush();
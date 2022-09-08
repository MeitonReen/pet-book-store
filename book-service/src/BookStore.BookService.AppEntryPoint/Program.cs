using BookStore.Base.DefaultSettings.ConfigurationManager;
using BookStore.Base.DefaultSettings.Logger;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.BookService.Settings.DefaultAppSettings;
using BookStore.BookService.Settings.DefaultRequestPipeline;
using BookStore.BookService.Settings.DiContainer;

var appBuilder = WebApplication.CreateBuilder(args);

appBuilder.Configuration
    .ApplyDefaultSettings(typeof(TargetAssemblyType))
    .ApplyDefaultLoggerSettings(appBuilder.Host);

await appBuilder.Services.AddDiSettings(appBuilder.Configuration);

var app = appBuilder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await InitializeData(app.Services);

app.UseRequestPipelineSettings();

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

    // GroupingInSpecsByApiVersions
    // var e = targetScopeServiceProvider
    //     .GetRequiredService<IActionDescriptorCollectionProvider>();
    // var partManager = targetScopeServiceProvider.GetRequiredService<ApplicationPartManager>();
    // var applicationParts = partManager.ApplicationParts.Select(x => x.Name);
    //
    // var controllerFeature = new ControllerFeature();
    // partManager.PopulateFeature(controllerFeature);
    //
    // var controllers = controllerFeature.Controllers.Select(x => x.Name);
    // ;
    // var minioInit = services.GetRequiredService<MinioInit>();
    // await minioInit.InitAsync();
}

namespace BookStore.BookService.AppEntryPoint
{
    public class Program
    {
    }
} //For targeting from integration tests
using BookStore.Base.DefaultSettings.ConfigurationManager;
using BookStore.BookService.IntegrationTests.Settings.DefaultAppSettings;
using BookStore.BookService.IntegrationTests.Settings.DiContainer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BookStore.BookService.IntegrationTests;

public class Startup
{
    private IConfiguration _configuration = default!;

    public void ConfigureHost(IHostBuilder hostBuilder)
    {
        hostBuilder
            .ConfigureHostConfiguration(sets => sets
                .LoadDefaultSettings(typeof(TargetAssemblyType), "Development", true));

        hostBuilder.ConfigureServices((hostBuilderContext, _) =>
            _configuration = hostBuilderContext.Configuration);
    }

    public void ConfigureServices(IServiceCollection services)
        => services.AddDiSettings(_configuration);
}
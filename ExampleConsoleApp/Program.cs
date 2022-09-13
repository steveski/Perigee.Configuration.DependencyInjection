using ExampleConsoleApp;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var env = hostBuilderContext.HostingEnvironment;
        services.RegisterAppSettings<Config>($"appsettings.{env.EnvironmentName}.json");
        
        services.AddTransient<App>();

    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();

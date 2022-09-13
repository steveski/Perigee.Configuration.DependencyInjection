using ExampleConsoleApp;
using ExampleConsoleEmbeddedResource;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration.DependencyInjection;
using System.Reflection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("ExampleConsoleEmbeddedResource.appsettings.json");
        services.RegisterAppSettingsFromStream<Config>(stream);

        services.AddTransient<App>();

    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();

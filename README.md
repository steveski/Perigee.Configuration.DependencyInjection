[![CI](https://github.com/steveski/Perigee.Configuration.DependencyInjection/actions/workflows/CI.yml/badge.svg)](https://github.com/steveski/Perigee.Configuration.DependencyInjection/actions/workflows/CI.yml)

[![Nuget](https://img.shields.io/nuget/v/Perigee.Configuration.DependencyInjection?label=Perigee.Configuration.DependencyInjection)](https://www.nuget.org/packages/Perigee.Configuration.DependencyInjection/)



# Perigee.Configuration.DependencyInjection
Provides a simple process for registering nested configuration types on an IServiceCollection. This is helpful when wanting dependency injection of configuration types loaded from JSON configuration.


### Installation
The package can be installed from NuGet using Install-Package Perigee.Configuration.DependencyInjection

### Usage
The most basic approach is when just using a single `appsettings.json` file for application configuration. All you need is to call the IServiceCollection extension method `RegisterAppSettings`. The following demonstrates registration in a .NET6.0 console application 
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        services.RegisterAppSettings<IConfig>();

        services.AddTransient<App>();
    })
    .Build();
    
var app = host.Services.GetRequiredService<App>();
app.Run();
```

If you are using appsettings override files named `appsettings.<environment>.json` then you can pass in the filename. Either explicitly specify the filename to use or derive the environment name from the hosting environment.
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        // Provide environment override appsettings file
        var env = hostBuilderContext.HostingEnvironment;
        services.RegisterAppSettings<IConfig>($"appsettings.{env.EnvironmentName}.json");

        services.AddTransient<App>();

    })
    .Build();

var app = host.Services.GetRequiredService<App>();
app.Run();
```

Note that `hostBuilderContext.HostingEnvironment` reads an environment variable which can be set for the visual studio configuration under `Properties\launchSettings.json`. Either ASPNETCORE_ENVIRONMENT or DOTNET_ENVIRONMENT will be read with ASPNETCORE_ENVIRONMENT taking precedence.
```json
{
  "profiles": {
    "ConsoleApp2": {
      "commandName": "Project",
      "environmentVariables": {
        "DOTNET_ENVIRONMENT": "Local"
      }
    }
  }
}
```

Because App has been registered with the `IServiceCollection` it can be injected with any types registered from the appsettings.json. Consider the following appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },

  "Database": {
    "ConnectionString": "prod.sqlite3",
    "SomethingElse": true
  }
}
```

`Logging` will be ignored for the purposes of service registration. To map the `Database` configuration, simply define your configuration structure using **pure interfaces**:

```csharp
public interface IConfig
{
    IDatabase Database { get; set; }
}

public interface IDatabase
{
    string ConnectionString { get; set; }
    bool SomethingElse { get; set; }
}
```

Because `RegisterAppSettings` recursively registers all nested interfaces, you can inject either the root `IConfig` or the specific leaf `IDatabase` interface into your services:

```csharp
public class App
{
    private readonly IDatabase _databaseConfig;

    public App(IDatabase databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public Task Run()
    {
        Console.Write(_databaseConfig.ConnectionString);

        return Task.CompletedTask;
    }

}
```

### Alternate scenarios
You can source json configuration fro registration from a Stream which is useful if your configuration is provided by some online service or an embedded resource, which is something you would use in a .NET MAUI application
```
    .
    .
    .
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var a = Assembly.GetExecutingAssembly();
        using var stream = a.GetManifestResourceStream("<Name Of Executable>.appsettings.json");
        services.RegisterAppSettingsFromStream<IConfig>(stream);

        services.AddTransient<App>();

    })
    .
    .
    .

    .
    .

```

### Performance & Best Practices
When you use this library, implementations for your interfaces are generated dynamically at runtime using `DispatchProxy`. These proxies are completely stateless and evaluate their properties against the hot-reloading `IConfigurationRoot` every time they are accessed.

For primitive types (`string`, `int`, `bool`), this is extremely fast. However, **nested interfaces** incur a performance penalty when accessed through their parent.
If you access `_config.Database.ConnectionString`, the proxy must use reflection to dynamically generate a *brand new* proxy instance for `IDatabase` on the fly. While this is perfectly fine for general application startup or web request handling, doing this inside a tight loop running thousands of times per second will cause CPU thrashing.

**Best Practice:** You should always lean toward injecting the specific **leaf objects** you need directly into your constructors. Because the library automatically registers all nested interfaces as singletons during startup, you can (should always where possible) bypass the root `IConfig` entirely:

```csharp
// GOOD: Injects the pre-built singleton proxy directly. Zero reflection overhead.
public App(IDatabase databaseConfig) { ... }

// OKAY: Accessing nested interfaces triggers dynamic proxy generation on every get.
public App(IConfig config) 
{ 
    var db = config.Database; 
}
```

### Changelog

**v2.0.0 (Major Architecture Shift)**
* **Pure Interfaces:** Concrete POCO classes are no longer required or supported. You now define your configuration strictly using `interface` definitions.
* **Dynamic Proxies:** The library now generates implementations for your interfaces at runtime using `System.Reflection.DispatchProxy`.
* **Instant Hot-Reloading:** Injected interfaces now dynamically evaluate their property getters against the underlying `IConfigurationRoot`. If `appsettings.json` is modified while the application is running, the injected interfaces (even Singletons!) instantly reflect the new values without requiring application restarts or `IOptionsMonitor`.
* **Removed `[EnvironmentOverride]`:** Standard `.NET` environment variable overriding now works natively through the core `IConfiguration` pipeline, making custom attributes obsolete.

This project is based on Rory Primrose's work https://github.com/roryprimrose/Divergic.Configuration.Autofac but removed the dependency on AutoFac sticking with the built in Microsoft DI.



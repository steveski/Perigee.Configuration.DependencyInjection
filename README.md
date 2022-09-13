![CI](https://github.com/steveski/Perigee.Configuration.DependencyInjection/workflows/CI/badge.svg)

[![CI](https://github.com/steveski/Perigee.Configuration.DependencyInjection/actions/workflows/CI.yml/badge.svg)](https://github.com/steveski/Perigee.Configuration.DependencyInjection/actions/workflows/CI.yml)

[![Nuget](https://img.shields.io/nuget/v/Perigee.Configuration.DependencyInjection?label=Perigee.Configuration.DependencyInjection)](https://www.nuget.org/packages/Perigee.Configuration.DependencyInjection/)



# Perigee.Configuration.DependencyInjection
Provides a simple process for registering nested configuration types on an IServiceCollection. This is helpful when wanting dependency injection of configuration types loaded from JSON configuration.


### Installation
The package can be installed from NuGet using Install-Package Perigee.Configuration.DependencyInjection

### Usage
The most basic approach is when just using a single `appsettings.json` file for application configuration. You need to create the registrar and then apply the resolved Json configuration to the IServiceCollection. The following demonstrates registration in a .NET6.0 console application 
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var registrar = new ConfigurationRegistrar<JsonResolver<Config>>();
        registrar.RegisterConfiguration(services);

        services.AddTransient<App>();
    })
    .Build();
    
var app = host.Services.GetRequiredService<App>();
app.Run();
```

If you are using appsettings override files named `appsettings.<environment>.json` then you can use the `EnvironmentJsonResolver`
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Perigee.Configuration;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostBuilderContext, services) =>
    {
        var env = hostBuilderContext.HostingEnvironment;

        var resolver = new EnvironmentJsonResolver<Config>("appsettings.json", $"appsettings.{env.EnvironmentName}.json");
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(services);

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
`Logging` will be ignored for the purposes of service registration so to register Database as itself as well as an interface of IDatabase then the following Config class object graph should be defined.
```csharp
public class Config
{
    public Database Database { get; set; }
}

public class Database : IDatabase // See comment below
{
    public string ConnectionString { get; set; }
    public bool SomethingElse { get; set; }
}

// Interface registration is entirely option and you can inject class directly
public interface IDatabase
{
    string ConnectionString { get; set; }
    bool SomethingElse { get; set; }
}
```
and the `App` class can received either the interface or class as an injected type
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
This project is based on Rory Primrose's work https://github.com/roryprimrose/Divergic.Configuration.Autofac but removed the dependency on AutoFac sticking with the built in Microsoft DI.



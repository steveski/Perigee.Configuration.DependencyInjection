namespace Perigee.Configuration.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Register the project's appsettings.json as injectable interface proxies
    /// </summary>
    /// <typeparam name="TInterface">The root interface which will represent the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    public static void RegisterAppSettings<TInterface>(this IServiceCollection serviceCollection) where TInterface : class
    {
        var resolver = new JsonResolver();
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration<TInterface>(serviceCollection);
    }

    /// <summary>
    /// Register the project's appsettings.json as injectable interface proxies
    /// </summary>
    /// <typeparam name="TInterface">The root interface which will represent the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<TInterface>(this IServiceCollection serviceCollection, string environmentFilename) where TInterface : class
    {
        var resolver = new EnvironmentJsonResolver(environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration<TInterface>(serviceCollection);
    }

    /// <summary>
    /// Register the project's appsettings.json as injectable interface proxies
    /// </summary>
    /// <typeparam name="TInterface">The root interface which will represent the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="filename">If you want to override the base appsettings.json file, pass it here</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<TInterface>(this IServiceCollection serviceCollection, string filename, string environmentFilename) where TInterface : class
    {
        var resolver = new EnvironmentJsonResolver(filename, environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration<TInterface>(serviceCollection);
    }

    /// <summary>
    /// Register the project's appsettings.json from an existing Stream. This is useful is for applications where the appsettings.json would be an embedded resource, such as .NET Maui.
    /// </summary>
    /// <typeparam name="TInterface">The root interface which will represent the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="stream">The stream containing the json configuration you would like to register</param>
    public static void RegisterAppSettings<TInterface>(this IServiceCollection serviceCollection, Stream? stream) where TInterface : class
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream), $"{nameof(RegisterAppSettings)} received a null parameter");

        var resolver = new StreamJsonResolver(stream);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration<TInterface>(serviceCollection);
    }
}

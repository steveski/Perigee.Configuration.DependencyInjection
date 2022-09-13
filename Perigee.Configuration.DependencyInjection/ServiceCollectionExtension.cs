namespace Perigee.Configuration.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="TConfig">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    public static void RegisterAppSettings<TConfig>(this IServiceCollection serviceCollection)
    {
        var registrar = new ConfigurationRegistrar<JsonResolver<TConfig>>();
        registrar.RegisterConfiguration(serviceCollection);

    }

    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="TConfig">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<TConfig>(this IServiceCollection serviceCollection, string environmentFilename)
    {
        var resolver = new EnvironmentJsonResolver<TConfig>(environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(serviceCollection);

    }

    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="TConfig">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="filename">If you want to override the base appsettings.json file, pass it here</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<TConfig>(this IServiceCollection serviceCollection, string filename, string environmentFilename)
    {
        var resolver = new EnvironmentJsonResolver<TConfig>(filename, environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(serviceCollection);

    }
}

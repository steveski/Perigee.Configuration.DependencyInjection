namespace Perigee.Configuration.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtension
{
    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="T">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    public static void RegisterAppSettings<T>(this IServiceCollection serviceCollection)
    {
        var registrar = new ConfigurationRegistrar<JsonResolver<T>>();
        registrar.RegisterConfiguration(serviceCollection);

    }

    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="T">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<T>(this IServiceCollection serviceCollection, string environmentFilename)
    {
        var resolver = new EnvironmentJsonResolver<T>(environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(serviceCollection);

    }

    /// <summary>
    /// Register the project's appsettings.json as injectable classes
    /// </summary>
    /// <typeparam name="T">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="filename">If you want to override the base appsettings.json file, pass it here</param>
    /// <param name="environmentFilename">The name of the overriding files, for example appsettings.Development.json</param>
    public static void RegisterAppSettings<T>(this IServiceCollection serviceCollection, string filename, string environmentFilename)
    {
        var resolver = new EnvironmentJsonResolver<T>(filename, environmentFilename);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(serviceCollection);

    }

    /// <summary>
    /// Register the project's appsettings.json from an existing Stream. This is useful is for applications where the appsettings.json would be an embedded resource, such as .NET Maui.
    /// </summary>
    /// <typeparam name="T">The type of the class which will represent the root of the appsettings.json file</typeparam>
    /// <param name="serviceCollection">An existing <see cref="IServiceCollection"/> where registrations will be added</param>
    /// <param name="stream">The stream containing the json configuration you would like to register</param>
    public static void RegisterAppSettings<T>(this IServiceCollection serviceCollection, Stream? stream)
    {
        if (stream is null) throw new ArgumentNullException(nameof(stream), $"{nameof(RegisterAppSettings)} received a null parameter");

        var resolver = new StreamJsonResolver<T>(stream);
        var registrar = new ConfigurationRegistrar(resolver);
        registrar.RegisterConfiguration(serviceCollection);

    }
}

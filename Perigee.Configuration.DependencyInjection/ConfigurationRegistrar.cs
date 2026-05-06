namespace Perigee.Configuration.DependencyInjection;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;

/// <summary>
/// The <see cref="ConfigurationRegistrar"/>
/// class is used to register nested configuration interface types dynamically.
/// </summary>
public class ConfigurationRegistrar
{
    private readonly IConfigurationResolver _resolver;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationRegistrar"/>.
    /// </summary>
    /// <param name="resolver">The configuration resolver.</param>
    public ConfigurationRegistrar(IConfigurationResolver resolver)
    {
        _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
    }

    /// <summary>
    /// Registers the configuration root and all nested interfaces.
    /// </summary>
    /// <typeparam name="TInterface">The root interface of the configuration.</typeparam>
    public void RegisterConfiguration<TInterface>(IServiceCollection serviceCollection) where TInterface : class
    {
        if (!typeof(TInterface).IsInterface)
        {
            throw new InvalidOperationException("Generic type TInterface must be an interface.");
        }

        var configuration = _resolver.Resolve();

        if (configuration is null)
        {
            return;
        }

        var registeredTypes = new HashSet<Type>();
        RegisterInterfaceRecursively(serviceCollection, typeof(TInterface), configuration, registeredTypes);
    }

    private void RegisterInterfaceRecursively(
        IServiceCollection serviceCollection,
        Type interfaceType,
        IConfiguration currentConfig,
        HashSet<Type> registeredTypes)
    {
        if (registeredTypes.Contains(interfaceType))
        {
            // We found a circular reference
            return;
        }

        registeredTypes.Add(interfaceType);

        // Register the proxy factory for this interface
        var proxyCreateMethod = typeof(ConfigurationDispatchProxy)
            .GetMethod(nameof(ConfigurationDispatchProxy.Create))!
            .MakeGenericMethod(interfaceType);

        serviceCollection.AddSingleton(interfaceType, sp =>
        {
            return proxyCreateMethod.Invoke(null, new object[] { currentConfig })!;
        });

        // Register all properties that return another interface
        var properties = interfaceType.GetProperties();

        foreach (var property in properties)
        {
            var returnType = property.PropertyType;

            // Only recurse into interfaces (excluding basic primitives/strings if they somehow slipped into the logic)
            if (returnType.IsInterface && returnType != typeof(string))
            {
                var section = currentConfig.GetSection(property.Name);
                RegisterInterfaceRecursively(serviceCollection, returnType, section, registeredTypes);
            }
        }
    }
}

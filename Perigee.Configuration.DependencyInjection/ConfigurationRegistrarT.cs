namespace Perigee.Configuration.DependencyInjection;

/// <summary>
/// The <see cref="ConfigurationRegistrar{T}"/>
/// class provides IServiceCollection registration support for nested configuration types.
/// </summary>
/// <typeparam name="T">The type of resolver that provides the root configuration.</typeparam>
public class ConfigurationRegistrar<T> : ConfigurationRegistrar
    where T : IConfigurationResolver, new()
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationRegistrar{T}"/>.
    /// </summary>
    public ConfigurationRegistrar()
        : base(new T())
    {
    }
}

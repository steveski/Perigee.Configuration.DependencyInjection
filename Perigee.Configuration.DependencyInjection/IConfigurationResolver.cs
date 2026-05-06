using Microsoft.Extensions.Configuration;

namespace Perigee.Configuration.DependencyInjection;

/// <summary>
/// The <see cref="IConfigurationResolver"/>
/// interface defines the members for resolving a configuration value.
/// </summary>
public interface IConfigurationResolver
{
    /// <summary>
    /// Resolves the root configuration.
    /// </summary>
    /// <returns>The configuration.</returns>
    IConfiguration? Resolve();
}

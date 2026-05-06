namespace Perigee.Configuration.DependencyInjection;

using Microsoft.Extensions.Configuration;
using System;

/// <summary>
/// The <see cref="JsonResolver"/>
/// class provides configuration support for loading the configuration from a json file.
/// </summary>
public class JsonResolver : IConfigurationResolver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="JsonResolver"/>.
    /// </summary>
    public JsonResolver()
    {
        JsonFilename = "appsettings.json";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonResolver"/>.
    /// </summary>
    /// <param name="filename">The filename of the json file to load.</param>
    public JsonResolver(string filename)
    {
        if (string.IsNullOrWhiteSpace(filename))
        {
            throw new ArgumentException("Filename is required", nameof(filename));
        }

        JsonFilename = filename;
    }

    /// <inheritdoc />
    public IConfiguration? Resolve()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(System.IO.Directory.GetCurrentDirectory())
            .AddJsonFile(JsonFilename, false, true);

        ConfigureBuilder(builder);

        var configurationRoot = builder.Build();

        return configurationRoot;
    }

    /// <summary>
    /// Configures the builder for resolving configuration data.
    /// </summary>
    /// <param name="builder">The builder.</param>
    protected virtual void ConfigureBuilder(IConfigurationBuilder builder)
    {
    }

    /// <summary>
    /// Gets the filename to load configuration values from.
    /// </summary>
    public virtual string JsonFilename { get; }
}

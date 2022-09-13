using Microsoft.Extensions.Configuration;

namespace Perigee.Configuration.DependencyInjection;

/// <summary>
/// The <see cref="StreamJsonResolver{T}"/> class provides configuration support for loading the configuration from a json stream.
/// </summary>
/// <typeparam name="T">The type of class to create from the configuration file.</typeparam>
public class StreamJsonResolver<T> : IConfigurationResolver
{
    private readonly Stream _stream;

    public StreamJsonResolver(Stream stream)
    {
        _stream = stream;
    }

    public object? Resolve()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonStream(_stream);

        ConfigureBuilder(builder);

        var configurationRoot = builder
            .Build();

        //var config = configurationRoot.Get(typeof(T));
        var config = configurationRoot.Get<T>();

        return config;
    }

    /// <summary>
    /// Configures the builder for resolving configuration data.
    /// </summary>
    /// <param name="builder">The builder.</param>
    protected virtual void ConfigureBuilder(IConfigurationBuilder builder)
    {
    }

}

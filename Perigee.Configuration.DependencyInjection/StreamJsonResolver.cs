using Microsoft.Extensions.Configuration;

namespace Perigee.Configuration.DependencyInjection;

/// <summary>
/// The <see cref="StreamJsonResolver"/> class provides configuration support for loading the configuration from a json stream.
/// </summary>
public class StreamJsonResolver : IConfigurationResolver
{
    private readonly Stream _stream;

    public StreamJsonResolver(Stream stream)
    {
        _stream = stream;
    }

    public IConfiguration? Resolve()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonStream(_stream);

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

}

using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Perigee.Configuration.DependencyInjection.IntegrationTests;

public interface ITestConfig
{
    string HotValue { get; set; }
}

public class AppSettingsHotReloadTests : IDisposable
{
    private readonly string _testFile;

    public AppSettingsHotReloadTests()
    {
        // We must use "appsettings.json" because the base JsonResolver constructor requires it
        // and throws if it is missing (optional: false).
        _testFile = "appsettings.json";
    }

    [Fact]
    public async Task Injected_Interface_Reflects_File_Changes()
    {
        // Arrange
        var initialJson = "{ \"HotValue\": \"Initial\" }";
        var updatedJson = "{ \"HotValue\": \"Updated\" }";

        await File.WriteAllTextAsync(_testFile, initialJson);

        var services = new ServiceCollection();
        // Since we explicitly added SetBasePath(Directory.GetCurrentDirectory()) earlier, this will work natively!
        services.RegisterAppSettings<ITestConfig>(_testFile);
        
        var serviceProvider = services.BuildServiceProvider();

        // Act & Assert (Initial)
        var config = serviceProvider.GetRequiredService<ITestConfig>();
        config.HotValue.ShouldBe("Initial");

        // Act (Update the file on disk)
        await File.WriteAllTextAsync(_testFile, updatedJson);

        // The underlying IConfiguration FileProvider usually takes a few milliseconds to detect and trigger the reload token
        await Task.Delay(1500);

        // Assert (The injected singleton proxy should now resolve the updated value!)
        config.HotValue.ShouldBe("Updated");
    }

    public void Dispose()
    {
        if (File.Exists(_testFile))
        {
            File.Delete(_testFile);
        }
    }
}

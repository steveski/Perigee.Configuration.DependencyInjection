namespace ExampleConsoleApp;

public class App
{
    private readonly IConfig _config;

    public App(IConfig config, IDatabase db)
    {
        _config = config;
    }

    public Task Run()
    {
        Console.WriteLine($"DB Connection is {_config.Database?.ConnectionString}");
        Console.WriteLine($"Boo: {_config.Database?.Boo}");
        Console.WriteLine($"Snootch: {_config.Database?.Snootch}");
        return Task.CompletedTask;
    }

}

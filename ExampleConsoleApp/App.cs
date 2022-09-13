namespace ExampleConsoleApp;

public class App
{
    private readonly IDatabase _databaseConfig;

    public App(IDatabase databaseConfig)
    {
        _databaseConfig = databaseConfig;
    }

    public Task Run()
    {
        Console.Write(_databaseConfig.ConnectionString);

        return Task.CompletedTask;
    }

}

namespace ExampleConsoleApp;

// You no longer need concrete classes! Just define the interfaces.
public interface IConfig
{
    IDatabase? Database { get; set; }
}

public interface IDatabase
{
    string? ConnectionString { get; set; }
    bool SomethingElse { get; set; }
    string? Snootch { get; set; }
    string? Boo { get; set; }
}

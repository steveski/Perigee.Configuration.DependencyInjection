namespace ExampleConsoleEmbeddedResource;

public class Config : IConfig
{
    public Database? Database { get; set; }
}

public class Database : IDatabase // See comment below
{
    public string? ConnectionString { get; set; }
    public bool SomethingElse { get; set; }
    public string? Snootch { get; set; }
}

// Interface registration is entirely option and you can inject class directly
public interface IConfig
{
    Database? Database { get; set; }
}

public interface IDatabase
{
    string? ConnectionString { get; set; }
    bool SomethingElse { get; set; }
    string? Snootch { get; set; }
}

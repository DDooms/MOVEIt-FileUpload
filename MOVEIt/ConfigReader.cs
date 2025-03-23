using System.Text.Json;

namespace MOVEIt;

public static class ConfigReader
{
    private static readonly string configPath = "Config.json";

    public static Config LoadConfig()
    {
        if (!File.Exists(configPath))
        {
            throw new FileNotFoundException($"Config file not found: {configPath}");
        }

        string json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<Config>(json) ?? throw new Exception("Invalid config file.");
    }
}

public class Config
{
    public string MoveItBaseUrl { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string LocalFolderPath { get; set; }
}
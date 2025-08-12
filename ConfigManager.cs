using System.Text.Json;

public class ConfigManager
{
    private const string ConfigPath = "config.json";

    public VlessConfig? LastConfig { get; set; }
    public bool AutoConnect { get; set; } = false;
    public bool StartWithSystem { get; set; } = false;

    public void Save()
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigPath, json);
    }

    public static ConfigManager Load()
    {
        if (!File.Exists(ConfigPath))
            return new ConfigManager();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<ConfigManager>(json) ?? new ConfigManager();
    }
}
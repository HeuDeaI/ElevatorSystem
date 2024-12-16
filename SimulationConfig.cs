using System;
using System.IO;
using System.Text.Json;

public class SimulationConfig
{
    public int TotalFloors { get; set; }
    public int ElevatorCount { get; set; }

    private SimulationConfig() { }

    public static SimulationConfig Default()
    {
        return new SimulationConfig
        {
            TotalFloors = 9,
            ElevatorCount = 2
        };
    }

    public static SimulationConfig LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Configuration file '{filePath}' not found.");

        string json = File.ReadAllText(filePath);
        var config = JsonSerializer.Deserialize<SimulationConfig>(json);

        return config ?? throw new InvalidOperationException("Invalid or empty configuration file.");
    }

    public static SimulationConfig GetConfiguration(string? configPath)
    {
        return string.IsNullOrWhiteSpace(configPath) 
            ? Default() 
            : LoadFromFile(configPath);
    }
}

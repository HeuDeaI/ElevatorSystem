using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

public class SimulationConfig
{
    public int TotalFloors { get; set; }
    public int ElevatorCount { get; set; }
    public PersonConfig[] People { get; set; } = Array.Empty<PersonConfig>();
    public SimulationConfig() { }

    [JsonConstructor]
    public SimulationConfig(int totalFloors, int elevatorCount, PersonConfig[] people)
    {
        TotalFloors = totalFloors;
        ElevatorCount = elevatorCount;
        People = people;
    }

    public static SimulationConfig Default()
    {
        return new SimulationConfig
        {
            TotalFloors = 9,
            ElevatorCount = 2,
            People = Array.Empty<PersonConfig>()
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

public class PersonConfig
{
    public int StartFloor { get; set; }
    public int TargetFloor { get; set; }
    public int TimeDelay { get; set; }

    public PersonConfig() { }

    [JsonConstructor]
    public PersonConfig(int startFloor, int targetFloor, int timeDelay)
    {
        StartFloor = startFloor;
        TargetFloor = targetFloor;
        TimeDelay = timeDelay;
    }
}

class Program
{
    static void Main()
    {
        Console.WriteLine("Enter the path to the configuration file (or press Enter to use default settings):");
        string? configPath = Console.ReadLine();

            SimulationConfig config = SimulationConfig.GetConfiguration(configPath);

            var building = new Building(config.TotalFloors, config.ElevatorCount);
            var display = new ElevatorDisplay(building);
            var controller = new SimulationController(building, display, config);

            controller.Run();
    }
}

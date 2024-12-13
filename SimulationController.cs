using System;
using System.Threading;

public class SimulationController
{
    private const int RenderDelay = 500;
    private readonly Building _building;
    private readonly ElevatorDisplay _display;
    private bool _isRunning = true;
    private bool _isPaused = false;

    public SimulationController(Building building, ElevatorDisplay display)
    {
        _building = building;
        _display = display;
    }

    public void Run()
    {
        var inputThread = new Thread(HandleHotkeys);
        inputThread.Start();

        while (true)
        {
            if (_isRunning)
            {
                if (!_isPaused)
                {
                    _display.Render();
                    Thread.Sleep(RenderDelay);
                }
            }
            else
            {
                Console.WriteLine("Simulation stopped. Press any key to exit.");
                break;
            }
        }
    }

    private void HandleHotkeys()
    {
        while (_isRunning)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;

                switch (key)
                {
                    case ConsoleKey.R:
                        _isPaused = false;
                        Console.WriteLine("Simulation resumed.");
                        break;

                    case ConsoleKey.P:
                        _isPaused = true;
                        Console.WriteLine("Simulation paused.");
                        break;

                    case ConsoleKey.S:
                        _isRunning = false;
                        Console.WriteLine("Stopping simulation...");
                        break;

                    case ConsoleKey.F:
                        _building.TriggerFireAlarm();
                        Console.WriteLine("Fire alarm triggered.");
                        break;

                    case ConsoleKey.A:
                        AddPerson();
                        break;

                    default:
                        Console.WriteLine("Unknown hotkey. Try again.");
                        break;
                }
            }
        }
    }

    private void AddPerson()
    {
        Console.WriteLine("Add Person: Enter start floor and end floor (e.g., 3 7):");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Invalid input. Try again.");
            return;
        }

        var args = input.Split(' ');
        if (args.Length == 2 &&
            int.TryParse(args[0], out int start) &&
            int.TryParse(args[1], out int end))
        {
            var person = new Person(start, end);
            if (end > start)
                person.RequestElevatorUp(_building);
            else
                person.RequestElevatorDown(_building);

            Console.WriteLine($"Person added: Start Floor {start}, End Floor {end}");
        }
        else
        {
            Console.WriteLine("Invalid input. Use format: <startFloor> <endFloor>");
        }
    }
}

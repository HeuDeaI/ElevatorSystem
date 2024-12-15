using System;
using System.Threading;

public class SimulationController
{
    private const int RenderDelay = 500;
    private readonly Building _building;
    private readonly ElevatorDisplay _display;
    private bool _isRunning = true;
    private bool _isPaused = false;
    private readonly Random _random = new Random();

    public SimulationController(Building building, ElevatorDisplay display)
    {
        _building = building;
        _display = display;
    }

    public void Run()
    {
        var inputThread = new Thread(Handlekeys);
        inputThread.Start();

        while (_isRunning)
        {
            if (!_isPaused)
            {
                _display.Render();
                Thread.Sleep(RenderDelay);

                if (_random.NextDouble() < 1.0 / 3.0)
                    AddRandomPerson();
            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    private void Handlekeys()
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
                        _building.ResumeAllElevators();
                        break;

                    case ConsoleKey.P:
                        _isPaused = true;
                        _building.PauseAllElevators();
                        break;

                    case ConsoleKey.S:
                        _isRunning = false;
                        _building.StopAllElevators();
                        break;

                    case ConsoleKey.F:
                        _building.TriggerFireAlarm();
                        break;

                    case ConsoleKey.A:
                        _isPaused = true;
                        _building.PauseAllElevators();
                        AddPerson();
                        _building.ResumeAllElevators();
                        _isPaused = false;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private void AddPerson()
    {
        Console.WriteLine("Add Person: Enter start floor and end floor (e.g., 3 7), or enter 0 for a random person:");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input) || input == "0")
        {
            AddRandomPerson();
            return;
        }

        var args = input.Split(' ');
        if (args.Length == 2 &&
            int.TryParse(args[0], out int start) &&
            int.TryParse(args[1], out int end))
        {
            if (start < 1 || start > _building.TotalFloors || end < 1 || end > _building.TotalFloors)
            {
                return;
            }

            if (start == end)
            {
                return;
            }

            new Thread(() =>
            {
                var person = new Person(start, end);
                if (end > start)
                    person.RequestElevatorUp(_building);
                else
                    person.RequestElevatorDown(_building);
            }).Start();
        }
    }

    private void AddRandomPerson()
    {
        int startFloor = _random.Next(1, _building.TotalFloors + 1);
        int targetFloor;

        do
        {
            targetFloor = _random.Next(1, _building.TotalFloors + 1);
        } while (targetFloor == startFloor);

        new Thread(() =>
        {
            var person = new Person(startFloor, targetFloor);
            if (targetFloor > startFloor)
                person.RequestElevatorUp(_building);
            else
                person.RequestElevatorDown(_building);
        }).Start();
    }
}

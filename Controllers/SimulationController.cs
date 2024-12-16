public class SimulationController
{
    private readonly Building _building;
    private readonly ElevatorDisplay _display;
    private readonly SimulationConfig _config;

    private const int RenderDelay = 500;
    private bool _isRunning = true;
    private bool _isPaused = false;
    private bool _availableToSpawn = true;
    private readonly Random _random = new Random();

    public SimulationController(Building building, ElevatorDisplay display, SimulationConfig config)
    {
        _building = building;
        _display = display;
        _config = config;
    }

    public void Run()
    {
        var inputThread = new Thread(HandleKeys);

        inputThread.Start();

        int configPersonIndex = 0;
        int timeIndex = 0;

        while (_isRunning)
        {
            if (!_isPaused)
            {
                _display.Render();
                Thread.Sleep(RenderDelay);

                if (_random.NextDouble() < 1.0 / 3.0 && _availableToSpawn)
                    AddRandomPerson();

                if (configPersonIndex < _config.People.Count() && _availableToSpawn)
                {
                    var personConfig = _config.People[configPersonIndex];
                    if (personConfig.TimeDelay < timeIndex / 2)
                        AddCustomPerson(personConfig.StartFloor, personConfig.TargetFloor);
                        configPersonIndex++;
                }
                timeIndex++;

            }
            else
            {
                Thread.Sleep(100);
            }
        }
    }

    private void HandleKeys()
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
                        _availableToSpawn = false;
                        _building.StopAllElevators();
                        _isRunning = false;
                        break;

                    case ConsoleKey.F:
                        _availableToSpawn = false;
                        _building.TriggerFireAlarm();
                        _availableToSpawn = true;
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

    private void AddCustomPerson(int startFloor, int targetFloor)
    {
        if (startFloor > _building.TotalFloors || targetFloor > _building.TotalFloors ||
            startFloor < 1 || targetFloor < 1 || startFloor == targetFloor)
        {
            return;
        }

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

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

public class Building
{
    public bool IsFireAlarmActive { get; private set; }
    private readonly ConcurrentDictionary<int, ConcurrentQueue<Person>> _upwardQueues;
    private readonly ConcurrentDictionary<int, ConcurrentQueue<Person>> _downwardQueues;
    private readonly ConcurrentBag<Elevator> _elevators;
    private readonly ConcurrentBag<Elevator> _availableElevators;
    private readonly ConcurrentQueue<Person> _requests;

    private int _fireAlarmCount;
    private TimeSpan _fireAlarmDuration;
    private Stopwatch _fireAlarmActiveTime;
    private readonly ConcurrentDictionary<int, TimeSpan> _personWaitTimes;
    private readonly Stopwatch _buildingClock;

    public int TotalFloors { get; }
    public IEnumerable<Elevator> Elevators => _elevators;

    public Building(int totalFloors, int elevatorCount)
    {
        TotalFloors = totalFloors < 20 ? totalFloors : 20;
        IsFireAlarmActive = false;
        _upwardQueues = new ConcurrentDictionary<int, ConcurrentQueue<Person>>();
        _downwardQueues = new ConcurrentDictionary<int, ConcurrentQueue<Person>>();
        _elevators = new ConcurrentBag<Elevator>();
        _availableElevators = new ConcurrentBag<Elevator>();
        _requests = new ConcurrentQueue<Person>();

        _fireAlarmCount = 0;
        _fireAlarmDuration = TimeSpan.Zero;
        _fireAlarmActiveTime = new Stopwatch();

        _personWaitTimes = new ConcurrentDictionary<int, TimeSpan>();
        _buildingClock = Stopwatch.StartNew();

        for (int floor = 0; floor <= TotalFloors; ++floor)
        {
            _upwardQueues[floor] = new ConcurrentQueue<Person>();
            _downwardQueues[floor] = new ConcurrentQueue<Person>();
        }

        elevatorCount = elevatorCount < 5 ? elevatorCount : 5;
        for (int i = 0; i < elevatorCount; i++)
        {
            var elevator = new Elevator(this);
            _elevators.Add(elevator);
            _availableElevators.Add(elevator);
        }
    }

    public void RequestElevatorUp(Person person)
    {
        person.WaitStartTime = _buildingClock.Elapsed;
        _upwardQueues.GetOrAdd(person.OriginFloor, _ => new ConcurrentQueue<Person>()).Enqueue(person);
        _requests.Enqueue(person);
        AssignElevatorToRequest();
    }

    public void RequestElevatorDown(Person person)
    {
        person.WaitStartTime = _buildingClock.Elapsed;
        _downwardQueues.GetOrAdd(person.OriginFloor, _ => new ConcurrentQueue<Person>()).Enqueue(person);
        _requests.Enqueue(person);
        AssignElevatorToRequest();
    }

    public void RecordPersonWaitTime(Person person)
    {
        var waitTime = _buildingClock.Elapsed - person.WaitStartTime;
        _personWaitTimes.AddOrUpdate(person.Id, waitTime, (id, existingTime) => waitTime);
    }

    public void TriggerFireAlarm()
    {
        IsFireAlarmActive = true;
        _fireAlarmCount++;
        _fireAlarmActiveTime.Restart();

        Parallel.ForEach(_upwardQueues.Values, queue => queue.Clear());
        Parallel.ForEach(_downwardQueues.Values, queue => queue.Clear());

        while (_requests.TryDequeue(out _)) { }

        Parallel.ForEach(_elevators, elevator => elevator.HandleFireAlarm());

        _fireAlarmActiveTime.Stop();
        _fireAlarmDuration += _fireAlarmActiveTime.Elapsed;

        IsFireAlarmActive = false;
   }

    private void AssignElevatorToRequest()
    {
        if (!_requests.TryPeek(out var person))
            return;

        var elevator = FindNearestAvailableElevator(person.OriginFloor);
        if (elevator != null && _availableElevators.TryTake(out var confirmedElevator))
        {
            _requests.TryDequeue(out _); 
            confirmedElevator.HandleRequest(person);
        }
    }

    public void RemovePersonFromRequests(Person person)
    {
        var remainingRequests = new ConcurrentQueue<Person>();

        while (_requests.TryDequeue(out var currentPerson))
        {
            if (!currentPerson.Equals(person))
            {
                remainingRequests.Enqueue(currentPerson); 
            }
        }

        foreach (var request in remainingRequests)
        {
            _requests.Enqueue(request);
        }
    }

    private Elevator? FindNearestAvailableElevator(int floor)
    {
        return _availableElevators
            .OrderBy(e => Math.Abs(e.CurrentFloor - floor))
            .FirstOrDefault();
    }

    public void ReleaseElevator(Elevator elevator)
    {
        _availableElevators.Add(elevator);
        AssignElevatorToRequest();
    }

    public void StopAllElevators()
    {
        Parallel.ForEach(_upwardQueues.Values, queue => queue.Clear());
        Parallel.ForEach(_downwardQueues.Values, queue => queue.Clear());

        while (_requests.TryDequeue(out _)) { }

        Parallel.ForEach(_elevators, elevator =>
        {
            elevator.Stop();
        });

        PrintElevatorStatistics();
    }

    public void PauseAllElevators()
    {
        foreach (var elevator in _elevators)
        {
            elevator.Pause();
        }
    }

    public void ResumeAllElevators()
    {
        foreach (var elevator in _elevators)
        {
            elevator.Resume();
        }
    }

    private void PrintElevatorStatistics()
    {
        int totalTrips = 0;
        int totalIdleTrips = 0;
        int totalDeliveredPersons = 0;

        string filePath = "ElevatorStatistics.txt";

        using (var writer = new StreamWriter(filePath))
        {
            writer.WriteLine("Elevator Statistics:");
            Console.WriteLine("Elevator Statistics:");

            foreach (var elevator in _elevators)
            {
                string elevatorStats = $"Elevator {elevator.Id}:\n" +
                                    $"  Total Trips: {elevator.TotalTrips}\n" +
                                    $"  Idle Trips: {elevator.IdleTrips}\n" +
                                    $"  Delivered Persons: {elevator.CountOfDeliveredPersons}\n";

                writer.WriteLine(elevatorStats);
                Console.WriteLine(elevatorStats);

                totalTrips += elevator.TotalTrips;
                totalIdleTrips += elevator.IdleTrips;
                totalDeliveredPersons += elevator.CountOfDeliveredPersons;
            }

            var totalWaitTime = _personWaitTimes.Values.Sum(t => t.TotalSeconds);
            var avgWaitTime = _personWaitTimes.Count > 0 ? totalWaitTime / _personWaitTimes.Count : 0;
            var longestWaitTime = _personWaitTimes.Values.Any() ? _personWaitTimes.Values.Max(t => t.TotalSeconds) : 0;

            string overallStats = "Overall Statistics:\n" +
                                  $"  Total Trips: {totalTrips}\n" +
                                  $"  Total Idle Trips: {totalIdleTrips}\n" +
                                  $"  Total Delivered Persons: {totalDeliveredPersons}\n" +
                                  $"  Fire Alarms Triggered: {_fireAlarmCount}\n" +
                                  $"  Total Fire Alarm Duration: {_fireAlarmDuration.TotalSeconds} seconds\n" +
                                  $"  Average Wait Time: {avgWaitTime} seconds\n" +
                                  $"  Longest Wait Time: {longestWaitTime} seconds";

            writer.WriteLine(overallStats);
            Console.WriteLine(overallStats);
        }

        Console.WriteLine($"Statistics saved to {filePath}");
    }

    public IDictionary<int, ConcurrentQueue<Person>> UpwardQueues => _upwardQueues;
    public IDictionary<int, ConcurrentQueue<Person>> DownwardQueues => _downwardQueues;
}

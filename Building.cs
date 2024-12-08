using System;
using System.Collections.Generic;

public class Building
{
    private static Building? _instance;
    private readonly Dictionary<int, Queue<Person>> _upwardQueues;
    private readonly Dictionary<int, Queue<Person>> _downwardQueues;
    private readonly List<Elevator> _elevators;
    private readonly List<Elevator> _availableElevators;
    private readonly List<Person> _requests;

    private Building()
    {
        _elevators = new List<Elevator>();
        _upwardQueues = new Dictionary<int, Queue<Person>>();
        _downwardQueues = new Dictionary<int, Queue<Person>>();
        _availableElevators = new List<Elevator>();
        _requests = new List<Person>();
    }

    public static Building Instance => _instance ??= new Building();

    public IReadOnlyList<Elevator> Elevators => _elevators.AsReadOnly();
    public IReadOnlyDictionary<int, Queue<Person>> UpwardQueues => _upwardQueues;
    public IReadOnlyDictionary<int, Queue<Person>> DownwardQueues => _downwardQueues;

    public void AddElevator(Elevator elevator)
    {
        _elevators.Add(elevator);
        _availableElevators.Add(elevator);
    }

    public void RequestElevatorUp(Person person)
    {
        EnqueueRequest(person, _upwardQueues);
        AssignElevatorToRequest(person);
    }

    public void RequestElevatorDown(Person person)
    {
        EnqueueRequest(person, _downwardQueues);
        AssignElevatorToRequest(person);
    }

    private void EnqueueRequest(Person person, Dictionary<int, Queue<Person>> queue)
    {
        if (!queue.ContainsKey(person.OriginFloor))
        {
            queue[person.OriginFloor] = new Queue<Person>();
        }
        queue[person.OriginFloor].Enqueue(person);
        _requests.Add(person);
    }

    private Elevator? FindNearestAvailableElevator(int floor)
    {
        Elevator? nearestElevator = null;
        int minDistance = int.MaxValue;

        foreach (var elevator in _availableElevators)
        {
            int distance = Math.Abs(elevator.CurrentFloor - floor);
            if (distance < minDistance)
            {
                nearestElevator = elevator;
                minDistance = distance;
            }
        }

        return nearestElevator;
    }

    private void AssignElevatorToRequest(Person person)
    {
        var elevator = FindNearestAvailableElevator(person.OriginFloor);
        if (elevator != null)
        {
            _availableElevators.Remove(elevator);
            elevator.MoveToFloorWithoutServing(person.OriginFloor);
            if (_requests.Any(p => p.Id == person.Id))
            {
                elevator.MoveToFloor(person.DestinationFloor);
            }
        }
    }

    public void AssignFreedElevator(Elevator elevator)
    {
        if (_requests.Count > 0)
        {
            var nextRequest = _requests[0];
            _requests.RemoveAt(0);
            _availableElevators.Remove(elevator);
            elevator.MoveToFloorWithoutServing(nextRequest.OriginFloor);
        }
        else
        {
            if (!_availableElevators.Contains(elevator))
            {
                _availableElevators.Add(elevator);
            }
        }
    }

    public void ReleaseElevator(Elevator elevator)
    {
        AssignFreedElevator(elevator);
    }
}

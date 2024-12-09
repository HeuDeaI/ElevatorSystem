using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

public class Building
{
    private readonly ConcurrentDictionary<int, ConcurrentQueue<Person>> _upwardQueues;
    private readonly ConcurrentDictionary<int, ConcurrentQueue<Person>> _downwardQueues;
    private readonly ConcurrentBag<Elevator> _elevators;
    private readonly ConcurrentBag<Elevator> _availableElevators;
    private readonly BlockingCollection<Person> _requests;

    public int TotalFloors { get; }
    public IEnumerable<Elevator> Elevators => _elevators;

    public Building(int totalFloors, int elevatorCount, int elevatorCapacity)
    {
        TotalFloors = totalFloors;
        _upwardQueues = new ConcurrentDictionary<int, ConcurrentQueue<Person>>();
        _downwardQueues = new ConcurrentDictionary<int, ConcurrentQueue<Person>>();
        _elevators = new ConcurrentBag<Elevator>();
        _availableElevators = new ConcurrentBag<Elevator>();
        _requests = new BlockingCollection<Person>();

        for (int i = 0; i < elevatorCount; i++)
        {
            var elevator = new Elevator(this, elevatorCapacity);
            _elevators.Add(elevator);
            _availableElevators.Add(elevator);
        }
    }

    public void RequestElevatorUp(Person person)
    {
        _upwardQueues.GetOrAdd(person.OriginFloor, _ => new ConcurrentQueue<Person>()).Enqueue(person);
        _requests.Add(person);
        AssignElevatorToRequest();
    }

    public void RequestElevatorDown(Person person)
    {
        _downwardQueues.GetOrAdd(person.OriginFloor, _ => new ConcurrentQueue<Person>()).Enqueue(person);
        _requests.Add(person);
        AssignElevatorToRequest();
    }

    private void AssignElevatorToRequest()
    {
        if (!_requests.TryTake(out var person))
            return;

        var elevator = FindNearestAvailableElevator(person.OriginFloor);
        if (elevator != null && _availableElevators.TryTake(out var confirmedElevator))
        {
            confirmedElevator.HandleRequest(person);
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

    public IDictionary<int, ConcurrentQueue<Person>> UpwardQueues => _upwardQueues;
    public IDictionary<int, ConcurrentQueue<Person>> DownwardQueues => _downwardQueues;
}

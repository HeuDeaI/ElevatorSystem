using System;
using System.Collections.Generic;

class Building
{
    private List<Elevator> _elevators;
    public Dictionary<int, Queue<Person>> QueueToUp { get; private set; }
    public Dictionary<int, Queue<Person>> QueueToDown { get; private set; }
    public List<Person> ListOfRequests { get; }
    public List<Elevator> FreeElevators { get; }

    private static Building? _instance;

    private Building()
    {
        _elevators = new List<Elevator>();
        QueueToUp = new Dictionary<int, Queue<Person>>();
        QueueToDown = new Dictionary<int, Queue<Person>>();
        ListOfRequests = new List<Person>();
        FreeElevators = new List<Elevator>();
    }

    public static Building Instance
    {
        get
        {
            return _instance ??= new Building();
        }
    }

    public void AddElevator(Elevator elevator)
    {
        _elevators.Add(elevator);
        FreeElevators.Add(elevator);
    }

    public void HandleUpRequest(Person person)
    {
        if (!QueueToUp.ContainsKey(person.SpawnFloor))
        {
            QueueToUp[person.SpawnFloor] = new Queue<Person>();
        }
        QueueToUp[person.SpawnFloor].Enqueue(person);
        ListOfRequests.Add(person);
        ManageElevators(person);
    }

    public void HandleDownRequest(Person person)
    {
        if (!QueueToDown.ContainsKey(person.SpawnFloor))
        {
            QueueToDown[person.SpawnFloor] = new Queue<Person>();
        }
        QueueToDown[person.SpawnFloor].Enqueue(person);
        ListOfRequests.Add(person);
        ManageElevators(person);
    }

    public void ManageElevators(Person person)
    {
        Elevator? nearestElevator = null;
        int smallestDistance = int.MaxValue;

        foreach (var elevator in FreeElevators)
        {
            int distance = Math.Abs(elevator.CurrentFloor - person.SpawnFloor);
            if (distance < smallestDistance)
            {
                nearestElevator = elevator;
                smallestDistance = distance;
            }
        }

        if (nearestElevator != null)
        {
            FreeElevators.Remove(nearestElevator);
            nearestElevator.MoveToFloorWithoutServing(person.SpawnFloor);
        }
    }

    public void AssignFreeElevator(Elevator elevator)
    {
        if (ListOfRequests[0] != null)
        {
            FreeElevators.Remove(elevator);
            elevator.MoveToFloorWithoutServing(ListOfRequests[0].SpawnFloor);
        }
    }
}
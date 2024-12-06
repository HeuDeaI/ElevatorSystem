using System;
using System.Collections.Generic;

public class Building
{
    public List<Elevator> Elevators { get; private set; }
    public Dictionary<int, Queue<Person>> QueueToUp { get; private set; }
    public Dictionary<int, Queue<Person>> QueueToDown { get; private set; }
    public List<Person> ListOfRequests { get; private set; }
    public List<Elevator> FreeElevators { get; private set; }

    private static Building? _instance;

    private Building()
    {
        Elevators = new List<Elevator>();
        QueueToUp = new Dictionary<int, Queue<Person>>();
        QueueToDown = new Dictionary<int, Queue<Person>>();
        ListOfRequests = new List<Person>();
        FreeElevators = new List<Elevator>();
    }

    public static Building Instance => _instance ??= new Building();

    public void AddElevator(Elevator elevator)
    {
        Elevators.Add(elevator);
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

    private Elevator? FindNearestElevator(int floor)
    {
        Elevator? nearestElevator = null;
        int smallestDistance = int.MaxValue;

        foreach (var elevator in FreeElevators)
        {
            int distance = Math.Abs(elevator.CurrentFloor - floor);
            if (distance < smallestDistance)
            {
                nearestElevator = elevator;
                smallestDistance = distance;
            }
        }

        return nearestElevator;
    }

    private void ManageElevators(Person person)
    {
        var nearestElevator = FindNearestElevator(person.SpawnFloor);
        if (nearestElevator != null)
        {
            FreeElevators.Remove(nearestElevator);
            nearestElevator.MoveToFloorWithoutServing(person.SpawnFloor);
        }
    }

    public void AssignFreeElevator(Elevator elevator)
    {
        if (ListOfRequests.Count > 0)
        {
            FreeElevators.Remove(elevator);
            elevator.MoveToFloorWithoutServing(ListOfRequests[0].SpawnFloor);
        }
    }
}

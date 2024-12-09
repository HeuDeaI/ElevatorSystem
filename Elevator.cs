using System;
using System.Collections.Generic;
using System.Threading;

public class Elevator
{
    private static int _idCounter = 0;
    private readonly Building _building;

    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public int? TargetFloor { get; private set; }
    public bool IsMovingUp { get; private set; }
    public int Capacity { get; }
    public List<Person> Passengers { get; }

    public Elevator(Building building, int capacity)
    {
        Id = Interlocked.Increment(ref _idCounter);
        _building = building;
        Capacity = capacity;
        Passengers = new List<Person>();
        CurrentFloor = 1;
    }

    public void HandleRequest(Person person)
    {
        MoveToFloorWithoutServing(person.OriginFloor);
        MoveToFloor(person.DestinationFloor);
    }

    public void MoveToFloorWithoutServing(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        IsMovingUp = destinationFloor > CurrentFloor;

        while (CurrentFloor != TargetFloor)
        {
            AdvanceOneFloor();
        }
    }

    public void MoveToFloor(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        IsMovingUp = destinationFloor > CurrentFloor;

        ServeCurrentFloor();
        while (CurrentFloor != TargetFloor)
        {
            AdvanceOneFloor();
            ServeCurrentFloor();
        }

        _building.ReleaseElevator(this);
    }

    private void AdvanceOneFloor()
    {
        Thread.Sleep(500);
        CurrentFloor += IsMovingUp ? 1 : -1;
    }

    private void ServeCurrentFloor()
    {
        DropOffPassengers();
        PickUpPassengers();
    }

    private void DropOffPassengers()
    {
        Passengers.RemoveAll(p => p.DestinationFloor == CurrentFloor);
    }

    private void PickUpPassengers()
    {
        var queue = IsMovingUp ? _building.UpwardQueues : _building.DownwardQueues;
        if (queue.TryGetValue(CurrentFloor, out var peopleQueue))
        {
            while (peopleQueue.TryDequeue(out var person) && Passengers.Count < Capacity)
            {
                Passengers.Add(person);
                UpdateTargetFloor(person);
            }
        }
    }

    private void UpdateTargetFloor(Person person)
    {
        if (IsMovingUp && person.DestinationFloor > TargetFloor)
        {
            TargetFloor = person.DestinationFloor;
        }
        else if (!IsMovingUp && person.DestinationFloor < TargetFloor)
        {
            TargetFloor = person.DestinationFloor;
        }
    }
}

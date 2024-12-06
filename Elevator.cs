using System;
using System.Collections.Generic;
using System.Threading;

public class Elevator
{
    private static int _idCounter = 0;
    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public int? TargetFloor { get; private set; }
    public bool MoveUp { get; set; }
    private Building _building;
    public int Capacity { get; private set; }
    public List<Person> Passengers { get; private set; }

    public Elevator(Building building, int capacity)
    {
        Id = ++_idCounter;
        _building = building;
        Capacity = capacity;
        Passengers = new List<Person>();
        CurrentFloor = 1;
    }

    public void MoveToFloor(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        while (CurrentFloor != TargetFloor)
        {
            ServeFloor();
            SwitchOneFloor();
        }

        if (!_building.FreeElevators.Contains(this))
        {
            _building.FreeElevators.Add(this);
        }
        _building.AssignFreeElevator(this);
    }

    private void ServeFloor()
    {
        DropOffPerson();
        BringPerson();
    }

    private void DropOffPerson()
    {
        Passengers.RemoveAll(person => person.DestinationFloor == CurrentFloor);
    }

    private void BringPerson()
    {
        var queue = MoveUp ? _building.QueueToUp : _building.QueueToDown;

        if (queue.ContainsKey(CurrentFloor))
        {
            while (queue[CurrentFloor].Count > 0 && Passengers.Count < Capacity)
            {
                var person = queue[CurrentFloor].Dequeue();
                _building.ListOfRequests.Remove(person);
                Passengers.Add(person);

                if (MoveUp && person.DestinationFloor > TargetFloor)
                {
                    TargetFloor = person.DestinationFloor;
                }
                else if (!MoveUp && person.DestinationFloor < TargetFloor)
                {
                    TargetFloor = person.DestinationFloor;
                }
            }
        }
    }

    public void MoveToFloorWithoutServing(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        MoveUp = TargetFloor > CurrentFloor;

        while (CurrentFloor != TargetFloor)
        {
            SwitchOneFloor();
        }

        BringPerson();
        if (TargetFloor != null)
        {
            MoveToFloor(TargetFloor.GetValueOrDefault());
        }
    }

    private void SwitchOneFloor()
    {
        Thread.Sleep(500);
        CurrentFloor += MoveUp ? 1 : -1;
    }
}

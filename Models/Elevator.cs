using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

public class Elevator
{
    private static int _idCounter = 0;
    private readonly Building _building;
    private const int _timeToAdvanceOneFloor = 500;

    public static int Capacity { get; set; } = 5; 
    public int Id { get; }
    public int CurrentFloor { get; private set; }
    public int? TargetFloor { get; private set; }
    public bool IsMovingUp { get; private set; }
    public List<Person> Passengers { get; }
    
    private ManualResetEventSlim _pauseEvent = new ManualResetEventSlim(true);
    private bool _isRunning = true;

    public Elevator(Building building)
    {
        Id = Interlocked.Increment(ref _idCounter);
        _building = building;
        Passengers = new List<Person>();
        CurrentFloor = 1;
    }

    public void HandleRequest(Person person)
    {
        MoveToFloorWithoutServing(person.OriginFloor);
        IsMovingUp = person.DestinationFloor > CurrentFloor;

        bool personIsWaiting = 
            _building.UpwardQueues[person.OriginFloor].Contains(person) ||
            _building.DownwardQueues[person.OriginFloor].Contains(person);

        ServeCurrentFloor();

        if (personIsWaiting)
        {
            MoveToFloor(person.DestinationFloor);
        }

        _building.ReleaseElevator(this);
    }

    public void MoveToFloorWithoutServing(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        IsMovingUp = destinationFloor > CurrentFloor;

        while (CurrentFloor != TargetFloor && _isRunning)
        {
            AdvanceOneFloor();
        }
    }

    public void HandleFireAlarm()
    {
        _pauseEvent.Wait();
        IsMovingUp = false;
        while(CurrentFloor != 1)
        {
            Thread.Sleep(_timeToAdvanceOneFloor);
            CurrentFloor += IsMovingUp ? 1 : -1;
        }
        Passengers.Clear();
    }

    public void MoveToFloor(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        while (CurrentFloor != TargetFloor && _isRunning)
        {
            AdvanceOneFloor();
            ServeCurrentFloor();
        }
    }


    private void AdvanceOneFloor()
    {
        if (_building.IsFireAlarmActive)
        {
            return;
        }
        _pauseEvent.Wait();
        Thread.Sleep(_timeToAdvanceOneFloor);
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
            while (Passengers.Count < Capacity && peopleQueue.TryDequeue(out var person))
            {
                _building.RemovePersonFromRequests(person);

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

    public void Pause()
    {
        _pauseEvent.Reset();
    }

    public void Resume()
    {
        _pauseEvent.Set();
    }

    public void Stop()
    {
        _isRunning = false;
        _pauseEvent.Set();
    }
}

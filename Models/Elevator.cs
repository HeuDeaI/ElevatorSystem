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
    public List<Person> ArrivingPassengers { get; private set; } = new List<Person>();

    
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
            if (_building.IsFireAlarmActive)
                return;
            AdvanceOneFloor();
        }
    }

    public void HandleFireAlarm()
    {
        _pauseEvent.Wait();
        while(CurrentFloor > 1)
        {
            Thread.Sleep(_timeToAdvanceOneFloor);
            CurrentFloor--;
        }
        
        ArrivingPassengers = Passengers.ToList();
        Passengers.Clear();
    }

    public void MoveToFloor(int destinationFloor)
    {
        TargetFloor = destinationFloor;
        while (CurrentFloor != TargetFloor && _isRunning)
        {
            if (_building.IsFireAlarmActive)
                return;
            AdvanceOneFloor();
            ServeCurrentFloor();
        }
    }

    public void MoveToLastFloors()
    {
        while (CurrentFloor != TargetFloor && CurrentFloor > 1 && CurrentFloor < _building.TotalFloors)
        {
            Thread.Sleep(_timeToAdvanceOneFloor);
            CurrentFloor += IsMovingUp ? 1 : -1;
            DropOffPassengers();
        }
    }


    private void AdvanceOneFloor()
    {
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
        ArrivingPassengers = Passengers.Where(p => p.DestinationFloor == CurrentFloor).ToList();
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
        _pauseEvent.Reset();

        if (TargetFloor.HasValue)
        {
            MoveToLastFloors();
        }

        _pauseEvent.Set();
    }
}

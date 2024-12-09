using System;

public class Person
{
    private static int _idCounter = 0;

    public int Id { get; }
    public int OriginFloor { get; }
    public int DestinationFloor { get; }

    public Person(int originFloor, int destinationFloor)
    {
        Id = Interlocked.Increment(ref _idCounter);
        OriginFloor = originFloor;
        DestinationFloor = destinationFloor;
    }

    public void RequestElevatorUp(Building building)
    {
        building.RequestElevatorUp(this);
    }

    public void RequestElevatorDown(Building building)
    {
        building.RequestElevatorDown(this);
    }
}

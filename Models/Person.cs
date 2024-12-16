using System.Threading;

public class Person
{
    private static int _idCounter = 0;

    public int Id { get; }
    public int OriginFloor { get; }
    public int DestinationFloor { get; }
    public int TimeDelay { get; }

    public Person(int originFloor, int destinationFloor, int timeDelay = 0)
    {
        Id = Interlocked.Increment(ref _idCounter);
        OriginFloor = originFloor;
        DestinationFloor = destinationFloor;
        TimeDelay = timeDelay;
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

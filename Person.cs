public class Person
{
    private static int _idCounter = 0;
    public int Id { get; }
    public int SpawnFloor { get; }
    public int DestinationFloor { get; }

    public Person(int spawnFloor, int destinationFloor)
    {
        Id = ++_idCounter;
        SpawnFloor = spawnFloor;
        DestinationFloor = destinationFloor;
    }

    public void CallElevatorToUp(Building building)
    {
        building.HandleUpRequest(this);
    }

    public void CallElevatorForDown(Building building)
    {
        building.HandleDownRequest(this);
    }
}

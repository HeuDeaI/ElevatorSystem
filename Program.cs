using System;

class Program
{
    static void Main()
    {
        var building = Building.Instance;

        building.AddElevator(new Elevator(building, 5));

        new Person(1, 5).CallElevatorToUp(building);
        new Person(7, 2).CallElevatorForDown(building);
    }
}

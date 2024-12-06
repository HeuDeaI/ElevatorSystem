using System;

class Program
{
    static void Main(string[] args)
    {
        var building = Building.Instance;
        building.AddElevator(new Elevator(building, 5));
        building.AddElevator(new Elevator(building, 5));

        var person1 = new Person(1, 5);
        var person2 = new Person(3, 1);
        var person3 = new Person(7, 2);

        person1.CallElevatorToUp(building);
        person2.CallElevatorForDown(building);
        person3.CallElevatorToUp(building);

        Console.WriteLine("Elevator system simulation completed.");
    }
}
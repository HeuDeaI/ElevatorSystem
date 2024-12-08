using System;
using System.Threading;

class Program
{
    static void Main()
    {
        var building = Building.Instance;

        building.AddElevator(new Elevator(building, 5));
        building.AddElevator(new Elevator(building, 5));
        building.AddElevator(new Elevator(building, 5));

        var display = new ElevatorDisplay(10, building);

        new Thread(() =>
        {
            Thread.Sleep(100);
            new Person(1, 5).RequestElevatorUp(building);

            Thread.Sleep(100);
            new Person(3, 7).RequestElevatorUp(building);

            // Add more calls as needed...
        }).Start();

        while (true)
        {
            display.Render();
            Thread.Sleep(50);
        }
    }
}

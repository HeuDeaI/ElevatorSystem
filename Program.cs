using System;
using System.Threading;

class Program
{
    static void Main()
    {
        var building = new Building(9, 3, 5);

        var display = new ElevatorDisplay(building);

        for (int i = 0; i < 10; i++)
        {
            int personId = i + 1;
            int startFloor = new Random().Next(1, 6);
            int targetFloor = new Random().Next(6, 11);

            new Thread(() =>
            {
                Thread.Sleep(personId * 1000);
                new Person(startFloor, targetFloor).RequestElevatorUp(building);
            }).Start();
        }

        while (true)
        {
            display.Render();
            Thread.Sleep(500);
        }
    }
}

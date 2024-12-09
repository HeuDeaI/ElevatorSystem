using System;
using System.Threading;

class Program
{
    private const int RenderDelay = 500;

    static void Main()
    {
        var building = new Building(totalFloors: 19, elevatorCount: 3);
        PersonManager.InitializePersonRequests(building, personCount: 20);
        var display = new ElevatorDisplay(building);

        while (true)
        {
            display.Render();
            Thread.Sleep(RenderDelay);
        }
    }
}

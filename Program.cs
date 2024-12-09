using System;
using System.Threading;

class Program
{
    private const int RenderDelay = 500; 

    static void Main()
    {
        var building = new Building(9, 1);

        PersonManager.InitializePersonRequests(building, personCount: 10);

        var display = new ElevatorDisplay(building);

        while (true)
        {
            display.Render();
            Thread.Sleep(RenderDelay);
        }
    }
}
